import { Button, Dialog, Flex, Modal, TextInput } from "@mantine/core";
import { useState } from "react";
import { useIdentityServerApi } from "../../api/IdentityServerApi";
import { useOrganizationApi } from "../../api/taskManagerApi";

type AddMemberToOrganizationProps = {
    organizationId: string;
    opened: boolean;
    onClose: () => void;
    onSuccess: () => void;
};

const AddMemberToOrganization = ({ organizationId, opened, onClose, onSuccess }: AddMemberToOrganizationProps) => {
    const { postInviteAccount } = useIdentityServerApi();
    const { postAddAccountToOrganization: addAccountToOrganization } = useOrganizationApi();
    const [email, setEmail] = useState('');
    const [firstName, setFirstName] = useState('');
    const [lastName, setLastName] = useState('');
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const handlerAddMember = async () => {
        if (!email || !firstName || !lastName || !organizationId) return;

        setLoading(true);
        const data = {
            email: email,
            firstName: firstName,
            lastName: lastName,
        };

        console.log("Adding member with data:", data);

        try {
            const addMemberToUm = await postInviteAccount(data);
            const AddMemberToOrganization = await addAccountToOrganization(
                organizationId,
                addMemberToUm.data.id!
            );

            if (addMemberToUm.status === 200 && AddMemberToOrganization.status === 200) {
                onSuccess()
                onClose()
            }
            else {
                setError("issue with adding member" + { data })
            }

            console.log("Member added to organization:", AddMemberToOrganization);
        } catch (error) {
            console.error("Failed to add member:", error);
        } finally {
            setLoading(false)
        }
    };

    return (
        <Modal opened={opened}
            withCloseButton
            onClose={onClose}
            size="lg"
            title="Add Member to Organization"
        >
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
            <Flex justify={"flex-end"} gap={"sm"}>
                <Button variant="filled" color="blue" onClick={handlerAddMember} loading={loading}>
                    Add Member
                </Button>
                <Button variant="outline" onClick={onClose} mr="md">
                    Cancel
                </Button>
            </Flex>
        </Modal>
    );
};

export default AddMemberToOrganization;
