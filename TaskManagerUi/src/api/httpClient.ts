import { useAuth } from 'react-oidc-context'

const taskManagerUrl = 'https://localhost:7099'

export const taskManagerApiClient = async (getToken: () => string | undefined, getOrganizationId: () => string | undefined) => {
  const request = async (path: string, options: RequestInit = {}) => {
    const token = getToken()
    const organizationId = getOrganizationId()
    const res = await fetch(`${taskManagerUrl}${path}`, {
      ...options,
      headers: {
        'Content-Type': 'application/json',
        Authorization: token ? `Bearer ${token}` : '',
        'organizationId': organizationId || '',
        ...(options.headers || {})
      },
    })
    if (!res.ok) throw new Error('API Error')
    return res.json()
  }

  return {
    get: (path: string) => request(path),
    post: (path: string, body: any) =>
      request(path, { method: 'POST', body: JSON.stringify(body) }),
    put: (path: string, body: any) =>
      request(path, { method: 'PUT', body: JSON.stringify(body) }),
    delete: (path: string) =>
      request(path, { method: 'DELETE' }),
  }
}
