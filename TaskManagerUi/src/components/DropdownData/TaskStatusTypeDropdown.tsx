import { Select } from "@mantine/core";

type TaskStatusTypeProps = {
  typeId: number;
  typeName: string;
  onChange?: (typeId: number) => void;
}

export const TaskStatusType: TaskStatusTypeProps[] = [
  { typeId: 1, typeName: 'To Do' },
  { typeId: 2, typeName: 'In Progress' },
  { typeId: 3, typeName: 'Done' }
];

export const TaskStatusTypeDropdown: React.FC<TaskStatusTypeProps> = ({ typeId, typeName, onChange}) => {
    const handleChange = (value: string | null) => {
    if (onChange && value) {
      onChange(Number(value));
      console.log(`Selected typeId: ${value}`);
    }
  };
  return (
    <Select
      data={TaskStatusType.map((status: TaskStatusTypeProps) => ({
        value: status.typeId.toString(),
        label: status.typeName,
      }))}
      value={typeId.toString()}
      onChange={handleChange}
    />
  );
};