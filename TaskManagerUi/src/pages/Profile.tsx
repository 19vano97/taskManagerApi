import { useEffect, useState } from "react"
import { useIdentityServerApi } from "../api/IdentityServerApi"
import { Card, Container, Fieldset, Title } from "@mantine/core"
import type { AccountDetails } from "../components/Types"
import { LoaderMain } from "../components/LoaderMain"

const Profile = () => {
    // Replace null with the appropriate acccountIds value if needed
    const { getAccountDetails } = useIdentityServerApi()
    const [accountDetails ,setAccountDetails] = useState<AccountDetails | null>(null)

    useEffect(() => {
        const fetchAccountDetails = async () => {
            try {
                const data = await getAccountDetails()
                setAccountDetails(data)
            } catch (error) {
                console.error("Error fetching account details:", error)
            }
        }

        fetchAccountDetails()
    }, [])

    return (
        <Container fluid>
            <Title order={1} mb="md" >
                Profile
            </Title>
            <Card shadow="sm" padding="lg" radius="md" withBorder>
                <h2>Your Profile</h2>
                <p>This is where you can view and manage your profile details.</p>
                {accountDetails ? (
                    <Fieldset legend="Account Details" mb="md">
                        <p><strong>Email:</strong> {accountDetails.email}</p>
                        <p><strong>First Name:</strong> {accountDetails.firstName}</p>
                        <p><strong>Last Name:</strong> {accountDetails.lastName}</p>
                    </Fieldset>
                ) : (
                    <LoaderMain />
                )}
            </Card>
        </Container>
    )
}
export default Profile