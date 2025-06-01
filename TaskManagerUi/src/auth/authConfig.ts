import { Log as oidcLog} from 'oidc-client-ts';

oidcLog.setLevel(oidcLog.DEBUG);

export const oidcConfig = {
    authority: "https://localhost:7270",
    client_id: "react-client",
    redirect_uri: "https://localhost:5173/signin-oidc",
    response_type: "code",
    scope: "openid profile api.read offline_access",
    post_logout_redirect_uri: "https://localhost:5173",
    automaticSilentRenew: true,
    loadUserInfo: true,
}