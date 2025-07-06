import { useEffect } from 'react';

export function useOrgLocalStorage(organizationId?: string | null) {
  useEffect(() => {
    if (!organizationId) return;

    const storedOrgId = localStorage.getItem('organizationId');
    if (storedOrgId !== organizationId) {
      localStorage.setItem('organizationId', organizationId);
    }
  }, [organizationId]);
}