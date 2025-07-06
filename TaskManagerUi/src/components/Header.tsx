import {
  Text,
  Container,
  Flex,
  Image,
  Select,
  Button,
  Burger,
  Menu,
  UnstyledButton,
  Group,
  Avatar,
  Loader,
  useMantineColorScheme,
} from '@mantine/core';
import { useEffect, useState } from 'react';
import { useSafeAuth } from '../hooks/useSafeAuth';
import { useInRouterContext, useNavigate } from 'react-router-dom';
import { ChevronDown, CircleUserRound, Moon, Sun } from 'lucide-react';
import classes from '../styles/Header.module.css';
import cx from 'clsx';
import type { AccountDetails } from './Types';
import { useIdentityServerApi } from '../api/IdentityServerApi';

export function Header({ opened, toggle }: { opened: boolean; toggle: () => void }) {
  const auth = useSafeAuth();
  const { getAccountDetails } = useIdentityServerApi();
  const [account, setAccount] = useState<AccountDetails>();
  const [userMenuOpened, setUserMenuOpened] = useState(false);
  const nav = useNavigate();
  const { colorScheme, setColorScheme } = useMantineColorScheme();
  const isDark = colorScheme === 'dark';
  const inRouter = useInRouterContext();

  if (!inRouter) return null;

  const fetchAccount = async () => {
    try {
      const data = await getAccountDetails();
      setAccount(data.data);
    } catch (err) {
      console.log(err);
    }
  };

  useEffect(() => {
    fetchAccount();
    const intervalId = setInterval(fetchAccount, 60 * 1000);
    return () => clearInterval(intervalId);
  }, []);

  const getInitials = (firstName: string, lastName: string) =>
    `${firstName?.[0] ?? ''}${lastName?.[0] ?? ''}`.toUpperCase();

  return (
    <Container fluid p="xs" style={{ display: 'flex', justifyContent: 'space-between', gap: '10px', flexWrap: 'wrap' }}>
      <Flex align="center" gap="xs">
        <Burger size="sm" color={isDark ? "white" : "black"} opened={opened} onClick={toggle} />
        <Flex
          align="center"
          gap="xs"
          style={{ textDecoration: 'none', cursor: 'pointer' }}
          onClick={() => nav('/')}
        >
          <Image src="/logo.svg" alt="Logo" w={28} h={28} />
          <Text
            size="lg"
            c={isDark ? "white" : "black"}
            style={{
              fontFamily: 'Segoe UI, sans-serif',
              fontWeight: 600,
            }}
          >
            TaskType
          </Text>
        </Flex>
      </Flex>

      <Flex justify="flex-end" align="center" gap="xs" style={{ flexWrap: 'wrap' }}>
        {auth?.isLoading ? (
          <Loader />
        ) : auth?.isAuthenticated && account ? (
          <Menu
            width={260}
            position="bottom-end"
            transitionProps={{ transition: 'pop-top-right' }}
            onClose={() => setUserMenuOpened(false)}
            onOpen={() => setUserMenuOpened(true)}
            withinPortal
          >
            <Menu.Target>
              <UnstyledButton
                className={cx(classes.user, { [classes.userActive]: userMenuOpened })}
              >
                <Group gap={7}>
                  <Avatar color="blue" radius="xl" size="md">
                    {getInitials(account.firstName, account.lastName)}
                  </Avatar>
                  <Flex direction="column" gap={0} align="flex-end">
                    <Text fw={500} size="sm" lh={1} style={{ maxWidth: 130, textAlign: 'right' }} lineClamp={1}>
                      {account.firstName} {account.lastName}
                    </Text>
                    <Text fw={400} size="xs" c="dimmed" lh={1} style={{ maxWidth: 130 }} lineClamp={1}>
                      {account.email}
                    </Text>
                  </Flex>
                  <ChevronDown size={12} />
                </Group>
              </UnstyledButton>
            </Menu.Target>
            <Menu.Dropdown>
              <Menu.Item
                component="a"
                href="/profile"
                leftSection={<CircleUserRound size={14} />}
              >
                Profile
              </Menu.Item>
              <Menu.Item
                onClick={() => setColorScheme(colorScheme === 'dark' ? 'light' : 'dark')}
                leftSection={colorScheme === 'dark' ? <Sun size={18} /> : <Moon size={18} />}
              >
                {colorScheme === 'dark' ? 'Light mode' : 'Dark mode'}
              </Menu.Item>
              <Menu.Divider />
              <Menu.Item onClick={() => auth.signoutRedirect()} color="red">
                Logout
              </Menu.Item>
            </Menu.Dropdown>
          </Menu>
        ) : (
          <Button onClick={() => auth.signinRedirect()}>Login</Button>
        )}
      </Flex>
    </Container>
  );
}