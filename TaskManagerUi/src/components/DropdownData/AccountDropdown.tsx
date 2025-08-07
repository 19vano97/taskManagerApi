import { useEffect, useState } from 'react';
import { Select } from '@mantine/core';
import { useOrganizationApi } from '../../api/taskManagerApi';
import type { AccountDetails } from '../Types';

type Props = {
  selectedAccount: AccountDetails | null;
  organizationId: string;
  placeholder: string;
  onAccountChange: (account: AccountDetails | null) => void;
};

export const AccountDropdown = ({
  selectedAccount,
  organizationId,
  placeholder,
  onAccountChange,
}: Props) => {
  const { getOrganizationAccounts } = useOrganizationApi();
  const [accounts, setAccounts] = useState<AccountDetails[]>([]);
  const [loading, setLoading] = useState(false);

  const fetchOrganizationAccounts = async () => {
    setLoading(true);
    try {
      const data = await getOrganizationAccounts(organizationId);
      setAccounts(data.data.accounts ?? []);
    } catch (err) {
      console.error("Error fetching accounts:", err);
    } finally {
      setLoading(false);
    }
  };

  // ✅ Optional auto-fetch on mount
  useEffect(() => {
    if (organizationId) fetchOrganizationAccounts();
  }, [organizationId]);

  const handleOnClick = () => {
    if (accounts.length === 0 && organizationId) {
      fetchOrganizationAccounts();
    }
  };

  return (
    <Select
      placeholder={placeholder}
      onClick={handleOnClick}
      data={accounts.map(a => ({
        value: a.id!,
        label: `${a.firstName} ${a.lastName}`,
      }))}
      value={selectedAccount?.id ?? null}
      onChange={(value) => {
        const account = accounts.find(a => a.id === value) || null;
        onAccountChange(account);
      }}
      searchable
      disabled={loading}
    />
  );
};