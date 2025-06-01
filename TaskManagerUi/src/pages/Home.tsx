import { Button, Container } from '@mantine/core';
import { LoaderMain } from '../components/LoaderMain';
import { useSafeAuth } from '../hooks/useSafeAuth';

function Home() {
    const auth = useSafeAuth();
    
    if (auth.isLoading) {
        return <LoaderMain />;
    }
    
    if (auth.isAuthenticated) {
        return (
            <Container>
                <p>Welcome back, {auth.user?.profile.email}!</p>
                <Button onClick={() => auth.signoutRedirect()}>Logout</Button>
            </Container>
     );
    }
    
    return (
        <Container>
            <h1>Welcome to the Task Manager</h1>
            <p>Please sign in to continue.</p>
            <Button onClick={() => auth.signinRedirect()}>Login</Button>
        </Container>
    );
}

export default Home;