import { Alert } from "@mantine/core"
import { Info } from "lucide-react"

export const WrongAlert = (text: string) => {
    const icon = <Info />
    return (
         <Alert variant="filled" color="red" radius="xl" withCloseButton title={text} icon={icon}>
      
        </Alert>
    )
}