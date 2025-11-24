import React from 'react';
import styles from './QuestionsListPage.module.css';
import { useNavigate, useParams } from 'react-router-dom';
import { deleteQuestion, getQuestionsForSurvey, SurveyQuestionItem, updateQuestionOrder } from '../services/questions';
import { ConfirmDialog } from '../components/ConfirmDialog';
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

export function QuestionsListPage() {
  const navigate = useNavigate();
  const params = useParams();
  const surveyId = Number(params.id);

  const [items, setItems] = React.useState<SurveyQuestionItem[]>([]);
  const [loading, setLoading] = React.useState<boolean>(true);
  const sensors = useSensors(useSensor(PointerSensor, { activationConstraint: { distance: 5 } }));
  const [confirmOpen, setConfirmOpen] = React.useState(false);
  const [targetId, setTargetId] = React.useState<number | null>(null);
  const [dirty, setDirty] = React.useState(false);
  const [saving, setSaving] = React.useState(false);

  React.useEffect(() => {
    let mounted = true;
    setLoading(true);
    getQuestionsForSurvey(surveyId)
      .then(list => {
        if (!mounted) return;
        // sort by orderIndex ascending
        setItems(list.slice().sort((a, b) => a.orderIndex - b.orderIndex));
      })
      .finally(() => {
        if (mounted) setLoading(false);
      });
    return () => { mounted = false; };
  }, [surveyId]);

  function handleDragEnd(event: DragEndEvent) {
    const { active, over } = event;
    if (!over || active.id === over.id) return;
    const oldIndex = items.findIndex(q => q.id === active.id);
    const newIndex = items.findIndex(q => q.id === over.id);
    if (oldIndex === -1 || newIndex === -1) return;
    const next = arrayMove(items, oldIndex, newIndex).map((q, idx) => ({ ...q, orderIndex: idx + 1 }));
    setItems(next);
    setDirty(true);
  }

  function SortableQuestion({ q }: { q: SurveyQuestionItem }) {
    const { attributes, listeners, setNodeRef, transform, transition } = useSortable({ id: q.id });
    const style: React.CSSProperties = {
      transform: transform ? `translate3d(${transform.x}px, ${transform.y}px, 0)` : undefined,
      transition,
    };
    return (
      <div ref={setNodeRef} style={style} className={styles.item}>
        <div className={styles.dragArea} {...attributes} {...listeners} title="Drag to reorder">
          <div className={styles.handle}>{q.orderIndex}</div>
          <div className={styles.text} title={q.questionText}>{q.questionText}</div>
        </div>
        <div className={styles.btnGroup}>
          <button className={styles.btn} onClick={() => navigate(`/forms/${surveyId}/questions/${q.id}/edit`)}>Edit</button>
          <button
            className={styles.btn}
            onClick={() => { setTargetId(q.id); setConfirmOpen(true); }}
          >
            Delete
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className={styles.root}>
      <div className={styles.container}>
        <h2 className={styles.title}>Questions</h2>
        <div className={styles.actions}>
          <button className={styles.btn} onClick={() => navigate(`/forms/${surveyId}/questions/create`)}>Add Question</button>
        </div>
        {loading ? (
          <div className={styles.empty}>Loading...</div>
        ) : items.length === 0 ? (
          <div className={styles.empty}>No questions yet.</div>
        ) : (
          <DndContext sensors={sensors} collisionDetection={closestCenter} onDragEnd={handleDragEnd}>
            <SortableContext items={items.map(q => q.id)} strategy={verticalListSortingStrategy}>
              <div className={styles.list}>
                {items.map((q) => (
                  <SortableQuestion key={q.id} q={q} />
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
                const ok = await updateQuestionOrder(surveyId, items.map(i => ({ id: i.id, orderIndex: i.orderIndex })));
                setSaving(false);
                if (ok) {
                  setDirty(false);
                } else {
                  alert('Saving order failed');
                }
              }}
            >
              {saving ? 'Saving...' : 'Save order of Questions'}
            </button>
          </div>
        )}
        <div style={{ marginTop: 16 }}>
          <button className={styles.btn} onClick={() => navigate(`/forms/${surveyId}/edit`)}>Back</button>
        </div>
      </div>
      <ConfirmDialog
        open={confirmOpen}
        title="Delete question"
        message="Are you sure you want to delete this question? This action cannot be undone."
        confirmLabel="Delete"
        cancelLabel="Cancel"
        onCancel={() => { setConfirmOpen(false); setTargetId(null); }}
        onConfirm={async () => {
          if (targetId == null) return;
          const ok = await deleteQuestion(targetId);
          if (ok) {
            setItems(prev => prev.filter(i => i.id !== targetId).map((it, idx) => ({ ...it, orderIndex: idx + 1 })));
          } else {
            alert('Delete failed');
          }
          setConfirmOpen(false);
          setTargetId(null);
        }}
      />
    </div>
  );
}


