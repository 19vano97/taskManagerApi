import { useEffect } from "react";
import { useAuth } from "react-oidc-context";
import { useNavigate } from "react-router-dom";

function SignInOidcHandler() {
  console.log("SignInOidcHandler component rendered");
  const auth = useAuth();
  const navigate = useNavigate();

  useEffect(() => {
    if (auth.isAuthenticated) {
      console.log("User is already authenticated:", auth.user);
      navigate("/kanban"); // Redirect if authenticated
    } else {
      console.log("Handling authentication callback...");
      auth.signinSilent()
        .then(() => navigate("/kanban")) // Refresh tokens and redirect
        .catch((error) => console.error("OIDC Sign-in Callback Error:", error));
    }
  }, [auth.isAuthenticated, navigate]);

  return <p>Processing login...</p>;
}

export default SignInOidcHandler;
