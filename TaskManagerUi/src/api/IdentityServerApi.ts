import { useEffect } from 'react';
import { useAuth } from 'react-oidc-context';
import {
  configureIdentityAxiosAuth,
  identityServerAxios,
} from './httpClient';
import type { AccountDetails } from '../components/Types';

export const useIdentityServerApi = () => {
  const auth = useAuth(); // используем напрямую, чтобы получить userManager

  useEffect(() => {
    configureIdentityAxiosAuth(
      identityServerAxios,
      () => auth.user?.access_token,
      () => auth.signoutRedirect()
    );
  }, [auth.user]);

  const defaultPath = '/api/auth';

  return {
    getAccountDetails: async () =>
      (await identityServerAxios.get<AccountDetails>(`${defaultPath}/details`)),
    postAccountData: async (data: AccountDetails) =>
      (await identityServerAxios.post<AccountDetails>(`${defaultPath}/details`, data)),
    postInviteAccount: async (data: AccountDetails) =>
      (await identityServerAxios.post<AccountDetails>(`${defaultPath}/invite`, data)),
  };
};