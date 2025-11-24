import React from 'react';
import styles from './QuestionElementsPage.module.css';
import { useNavigate, useParams } from 'react-router-dom';
import { changeQuestionElementOrder, deleteQuestionElement, getQuestionElements, QuestionElementItem } from '../services/questions';
import {
  DndContext,
  closestCenter,
  PointerSensor,
  useSensor,
  useSensors,
  DragEndEvent,
} from '@dnd-kit/core';
import {
  arrayMove,
  SortableContext,
  verticalListSortingStrategy,
  useSortable,
} from '@dnd-kit/sortable';
import { ConfirmDialog } from '../components/ConfirmDialog';

export function QuestionElementsPage() {
  const navigate = useNavigate();
  const params = useParams();
  const surveyId = Number(params.id);
  const questionId = Number(params.questionId);

  const [items, setItems] = React.useState<QuestionElementItem[]>([]);
  const [loading, setLoading] = React.useState(true);
  const sensors = useSensors(useSensor(PointerSensor, { activationConstraint: { distance: 5 } }));
  const [confirmOpen, setConfirmOpen] = React.useState(false);
  const [targetId, setTargetId] = React.useState<number | null>(null);
  const [dirty, setDirty] = React.useState(false);
  const [saving, setSaving] = React.useState(false);

  React.useEffect(() => {
    let mounted = true;
    setLoading(true);
    getQuestionElements(questionId)
      .then(list => {
        if (!mounted) return;
        setItems(list.slice().sort((a, b) => a.orderIndex - b.orderIndex));
      })
      .finally(() => mounted && setLoading(false));
    return () => { mounted = false; };
  }, [questionId]);

  function handleDragEnd(event: DragEndEvent) {
    const { active, over } = event;
    if (!over || active.id === over.id) return;
    const oldIndex = items.findIndex(e => e.id === active.id);
    const newIndex = items.findIndex(e => e.id === over.id);
    if (oldIndex === -1 || newIndex === -1) return;
    const next = arrayMove(items, oldIndex, newIndex).map((e, idx) => ({ ...e, orderIndex: idx + 1 }));
    setItems(next);
    setDirty(true);
  }

  function SortableElement({ el }: { el: QuestionElementItem }) {
    const { attributes, listeners, setNodeRef, transform, transition } = useSortable({ id: el.id });
    const style: React.CSSProperties = {
      transform: transform ? `translate3d(${transform.x}px, ${transform.y}px, 0)` : undefined,
      transition,
    };
    return (
      <div ref={setNodeRef} style={style} className={styles.item}>
        <div className={styles.dragArea} {...attributes} {...listeners} title="Drag to reorder">
          <div className={styles.handle}>{el.orderIndex}</div>
          <div className={styles.text}>{el.svgName}</div>
        </div>
        <div className={styles.btnGroup}>
          <button className={styles.btn} onClick={() => navigate(`/forms/${surveyId}/questions/${questionId}/elements/${el.id}/edit`)}>Edit</button>
          <button className={styles.btn} onClick={() => { setTargetId(el.id); setConfirmOpen(true); }}>Delete</button>
        </div>
      </div>
    );
  }

  return (
    <div className={styles.root}>
      <div className={styles.container}>
        <h2 className={styles.title}>Elements</h2>
        <div className={styles.toolbar}>
          <button className={styles.btn} onClick={() => navigate(`/forms/${surveyId}/questions/${questionId}/edit`)}>Back</button>
          <button className={styles.btn} onClick={() => navigate(`/forms/${surveyId}/questions/${questionId}/elements/create`)}>Add Element</button>
        </div>
        {loading ? (
          <div className={styles.placeholder}>Loading...</div>
        ) : items.length === 0 ? (
          <div className={styles.placeholder}>No elements yet.</div>
        ) : (
          <DndContext sensors={sensors} collisionDetection={closestCenter} onDragEnd={handleDragEnd}>
            <SortableContext items={items.map(e => e.id)} strategy={verticalListSortingStrategy}>
              <div className={styles.list}>
                {items.map(el => (
                  <SortableElement key={el.id} el={el} />
                ))}
              </div>
            </SortableContext>
          </DndContext>
        )}
        {dirty && (
          <div className={styles.saveBar}>
            <button
              className={styles.btn}
              disabled={saving}
              onClick={async () => {
                setSaving(true);
                const ok = await changeQuestionElementOrder(items.map(i => ({ id: i.id, orderIndex: i.orderIndex })));
                setSaving(false);
                if (ok) {
                  setDirty(false);
                } else {
                  alert('Saving element order failed');
                }
              }}
            >
              {saving ? 'Saving...' : 'Save order of Elements'}
            </button>
          </div>
        )}
        <ConfirmDialog
          open={confirmOpen}
          title="Delete element"
          message="Are you sure you want to delete this element? This action cannot be undone."
          confirmLabel="Delete"
          cancelLabel="Cancel"
          onCancel={() => { setConfirmOpen(false); setTargetId(null); }}
          onConfirm={async () => {
            if (targetId == null) return;
            const ok = await deleteQuestionElement(targetId);
            if (ok) {
              setItems(prev => {
                const next = prev
                  .filter(i => i.id !== targetId)
                  .map((it, idx) => ({ ...it, orderIndex: idx + 1 }));
                // persist new order
                changeQuestionElementOrder(next.map(i => ({ id: i.id, orderIndex: i.orderIndex }))).catch(() => {});
                return next;
              });
            } else {
              alert('Delete failed');
            }
            setConfirmOpen(false);
            setTargetId(null);
          }}
        />
      </div>
    </div>
  );
}


