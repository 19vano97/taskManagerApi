import React from 'react';
import { Text } from "@mantine/core";
import { useFormattedDate } from '../../hooks/useFormattedDate';

type TaskDateProps = {
  dateString: string;
};

const TaskDate: React.FC<TaskDateProps> = ({ dateString }) => {
  const formattedDate = useFormattedDate(dateString);

  return (<Text c={"gray.7"} size='xs'>{formattedDate}</Text>);
};

export default TaskDate;