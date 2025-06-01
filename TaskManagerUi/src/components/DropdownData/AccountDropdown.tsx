import React from 'react';
import { Select, Fieldset } from '@mantine/core';
import type { AccountDetails } from '../Types';

type AccountDropdownProps = {
  selectedAccount: AccountDetails | null;
  accounts: AccountDetails[];
  placeholder: string;
  onAccountChange: (account: AccountDetails | null) => void;
};

export const AccountDropdown: React.FC<AccountDropdownProps> = ({ selectedAccount, accounts, placeholder, onAccountChange }) => {
  const handleAccountChange = (value: string | null) => {
    const selected = accounts.find((account) => account.id === value) || null;
    onAccountChange(selected);
  };

  return (
      <Select
        placeholder={placeholder}
        data={accounts.map((account) => ({
          value: account.id,
          label: account.email,
        }))}
        value={selectedAccount ? selectedAccount.id : null}
        onChange={handleAccountChange}
        searchable
        style={{ width: '100%' }}
      />
  );
};