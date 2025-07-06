import { Alert } from '@mantine/core';
import { CircleCheck } from 'lucide-react';

type SuccessAlertProps = {
    title?: string;
    onClose?: () => void;
}

const SuccessAlert = (prop: SuccessAlertProps) => {
    const icon = <CircleCheck />;
    return (
        <Alert variant="light" color="cyan" title={prop.title} icon={icon}>

        </Alert>
    );
}

export default SuccessAlert;