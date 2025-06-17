import React from 'react';
import { useFormattedDate } from '../../hooks/useFormattedDate';

type TaskDateProps = {
  dateString: string;
};

const TaskDate: React.FC<TaskDateProps> = ({ dateString }) => {
  const formattedDate = useFormattedDate(dateString);

  return <span>{formattedDate}</span>;
};

export default TaskDate;