import { Button, Container, Group, Image, Stack, Text, Title } from '@mantine/core';
import { useNavigate } from 'react-router-dom';

export default function NotFoundPage() {
  const navigate = useNavigate();

  return (
    <Container size="md" py="xl" style={{ textAlign: 'center' }}>
      <Stack align="center">
        <Title order={2}>Page not found</Title>
        <Text c="dimmed">
          The page you are looking for doesn't exist. It might have been moved or deleted.
        </Text>

        <Group justify="center">
          <Button variant="light" onClick={() => navigate('/')}>
            Take me back home
          </Button>
        </Group>
      </Stack>
    </Container>
  );
}
