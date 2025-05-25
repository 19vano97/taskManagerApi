import { useAuth } from 'react-oidc-context';

function Home(){
    const auth = useAuth();
    
    if (auth.isLoading) {
        return <p>Loading...</p>;
    }
    
    if (auth.isAuthenticated) {
        return (
            <div>
                <p>Welcome back, {auth.user?.profile.email}!</p>
                <button onClick={() => auth.signoutRedirect()}>Logout</button>
            </div>
     );
    }
    
    return (
        <div>
            <h1>Welcome to the Task Manager</h1>
            <p>Please sign in to continue.</p>
            <button onClick={() => auth.signinRedirect()}>Login</button>
        </div>
    );
}
export default Home