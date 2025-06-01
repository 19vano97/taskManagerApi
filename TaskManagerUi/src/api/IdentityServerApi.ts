import { identityServerApiClient } from "./httpClient";
import { useSafeAuth } from "../hooks/useSafeAuth";

export const useIdentityServerApi = () => {
    const { user } = useSafeAuth();;
    const defaultPath = '/api/auth';
    const apiPromise = identityServerApiClient(() => user?.access_token);

    return {
        getAccountDetails: async () =>
            (await apiPromise).get(`${defaultPath}/details`),

        getAllAccountDetails: async (accountIds: string[]) =>
            (await apiPromise).post(`${defaultPath}/details/accounts`, accountIds),


    };
};
