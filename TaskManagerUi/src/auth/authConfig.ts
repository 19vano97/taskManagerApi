// src/auth/authConfig.ts
import { Log as oidcLog, WebStorageStateStore } from 'oidc-client-ts';
import type { AuthProviderProps } from 'react-oidc-context';

oidcLog.setLevel(oidcLog.DEBUG);

export const oidcConfig: Partial<AuthProviderProps> = {
  authority:               "https://localhost:7270",
  client_id:               "react-client",
  redirect_uri:            `${window.location.origin}/signin-oidc`,
  silent_redirect_uri:     `${window.location.origin}/silent-renew.html`,
  response_type:           "code",
  scope:                   "openid profile api.read offline_access",
  post_logout_redirect_uri:`${window.location.origin}`,
  userStore:               new WebStorageStateStore({ store: window.localStorage }),
  automaticSilentRenew:    true,

};