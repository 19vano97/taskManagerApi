import { Button, Dialog, TextInput } from "@mantine/core";
import { useDisclosure } from "@mantine/hooks";
import { useState } from "react";
import { useIdentityServerApi } from "../../api/IdentityServerApi";
import { useOrganizationApi } from "../../api/taskManagerApi";

type AddMemberToOrganizationProps = {
  organizationId: string;
};

const AddMemberToOrganization = (props: AddMemberToOrganizationProps) => {
    const { postInviteAccount } = useIdentityServerApi();
    const { postAddAccountToOrganization: addAccountToOrganization } = useOrganizationApi();
    const [opened, { toggle, close }] = useDisclosure(false);
    const [email, setEmail] = useState('');
    const [firstName, setFirstName] = useState('');
    const [lastName, setLastName] = useState('');

    const handlerAddMember = async () => {
        const data = {
            email: email,
            firstName: firstName,
            lastName: lastName,
        };

        console.log("Adding member with data:", data);

        try {
            const addMemberToUm = await postInviteAccount(data);
            const AddMemberToOrganization = await addAccountToOrganization(
                props.organizationId,
                addMemberToUm.data.id!
            );

            console.log("Member added to organization:", AddMemberToOrganization);
        } catch (error) {
            console.error("Failed to add member:", error);
        }

        close();
    };

    return (
        <Dialog opened={true} withCloseButton onClose={close} size="lg" title="Add Member to Organization">
            <TextInput
                label="Email"
                placeholder="Enter the email of the member to add"
                required
                mb="md"
                value={email}
                onChange={(e) => setEmail(e.currentTarget.value)}
            />
            <TextInput
                label="First Name"
                placeholder="Enter the first name of the member to add"
                required
                mb="md"
                value={firstName}
                onChange={(e) => setFirstName(e.currentTarget.value)}
            />
            <TextInput
                label="Last Name"
                placeholder="Enter the last name of the member to add"
                required
                mb="md"
                value={lastName}
                onChange={(e) => setLastName(e.currentTarget.value)}
            />
            <Button variant="filled" color="blue" onClick={handlerAddMember}>
                Add Member
            </Button>
        </Dialog>
    );
};

export default AddMemberToOrganization;
