import axios from 'axios';
type AxiosInstance = ReturnType<typeof axios.create>;

export const taskManagerAxios: AxiosInstance = axios.create({
  baseURL: 'https://localhost:7099',
  headers: {
    'Content-Type': 'application/json',
  },
});

export const identityServerAxios: AxiosInstance = axios.create({
  baseURL: 'https://localhost:7270',
  headers: {
    'Content-Type': 'application/json',
  },
});

export function configureAxiosAuth(
  axiosInstance: AxiosInstance,
  getToken: () => string | undefined,
  getOrganizationId?: () => string | undefined,
 
  onUnauthorized?: () => void,
  userManager?: { signinSilent: () => Promise<any> }
) {
  axiosInstance.interceptors.request.use((config) => {
    const token = getToken();
    if (token) {
      config.headers = config.headers || {};
      config.headers.Authorization = `Bearer ${token}`;
    }

    const orgId = getOrganizationId?.();
    if (orgId) {
      config.headers = config.headers || {};
      config.headers['organizationId'] = orgId;
    }



    console.log('[Request]', config.method?.toUpperCase(), config.url);
    return config;
  }, (error) => {
    console.error('[Request Error]', error);
    return Promise.reject(error);
  });

  axiosInstance.interceptors.response.use(
    (response) => {
      console.log('[Response]', response.status, response.config.url);
      return response;
    },
    async (error) => {
      const originalRequest = error.config;
      const status = error.response?.status;

      console.error('[Response Error]', status, originalRequest?.url);

      if ((status === 401 || status === 403) && !originalRequest._retry) {
        originalRequest._retry = true;
        try {
          if (userManager && typeof userManager.signinSilent === 'function') {
            const user = await userManager.signinSilent();
            const newToken = user && (user.access_token || user.id_token);
            if (newToken) {
              // Обновляем заголовки с новым токеном
              axiosInstance.defaults.headers.common['Authorization'] = `Bearer ${newToken}`;
              if (!originalRequest.headers) originalRequest.headers = {};
              originalRequest.headers.Authorization = `Bearer ${newToken}`;
              // Повторяем запрос
              return axiosInstance(originalRequest);
            } else {
              throw new Error('No token received from signinSilent');
            }
          }
          if (onUnauthorized) {
            onUnauthorized();
          } else {
            window.location.href = '/signin-oidc';
          }
        } catch (refreshError) {
          console.error('Token refresh failed:', refreshError);
          if (onUnauthorized) {
            onUnauthorized();
          } else {
            window.location.href = '/signin-oidc';
          }
        }
      }

      return Promise.reject(error);
    }
  );
}


export function configureIdentityAxiosAuth(
  axiosInstance: AxiosInstance,
  getToken: () => string | undefined,
  onUnauthorized?: () => void
) {
  axiosInstance.interceptors.request.use((config) => {
    const token = getToken();
    if (token) {
      config.headers = config.headers || {};
      config.headers.Authorization = `Bearer ${token}`;
    }

    console.log('[Identity Request]', config.method?.toUpperCase(), config.url);
    return config;
  }, (error) => {
    console.error('[Identity Request Error]', error);
    return Promise.reject(error);
  });

  axiosInstance.interceptors.response.use(
    (response) => {
      console.log('[Identity Response]', response.status, response.config.url);
      return response;
    },
    async (error) => {
      const status = error.response?.status;
      console.error('[Identity Response Error]', status);

      if ((status === 401 || status === 403)) {
        if (onUnauthorized) {
          onUnauthorized();
        } else {
          window.location.href = '/signin-oidc';
        }
      }

      return Promise.reject(error);
    }
  );
}