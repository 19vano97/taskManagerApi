import React, { useEffect, useState } from 'react';
import { Select, Fieldset } from '@mantine/core';
import type { AccountDetails } from '../Types';
import { useOrganizationApi } from '../../api/taskManagerApi';

type AccountDropdownProps = {
  selectedAccount: AccountDetails | null;
  organizationId: string;
  placeholder: string;
  onAccountChange: (account: AccountDetails | null) => void;
};

export const AccountDropdown: React.FC<AccountDropdownProps> = ({ selectedAccount, organizationId, placeholder, onAccountChange }) => {
  const { getOrganizationAccounts } = useOrganizationApi();
  const [accountsLoading, setAccountsLoading] = useState<boolean>(false);
  const [accounts, setAccounts] = useState<AccountDetails[]>([]);

  const fetchOrganizationAccounts = async () => {
    setAccountsLoading(true);
    try {
      const data = await getOrganizationAccounts(organizationId!);
      setAccounts(data.data.accounts || []);
    } catch (error) {
      console.error('Error fetching accounts:', error);
    } finally {
      setAccountsLoading(false);
    }
  };

  useEffect(() => {
    if (organizationId && !accounts) {
      fetchOrganizationAccounts();
    }
  }, []);

  const handleOnClick = () => {
    if (!accounts || accounts.length === 0) {
      fetchOrganizationAccounts();
    }
  }

  const handleAccountChange = async (value: string | null) => {
    const selected = accounts.find((account) => account.id === value) || null;
    onAccountChange(selected);
  };

  return (
    <Select
      onClick={handleOnClick}
      placeholder={placeholder}
      data={accounts.map((account) => ({
        value: account.id!,
        label: account.firstName + " " + account.lastName,
      }))}
      value={selectedAccount ? selectedAccount.id : null}
      onChange={handleAccountChange}
      searchable
      style={{ width: '100%' }}
    />
  );
};