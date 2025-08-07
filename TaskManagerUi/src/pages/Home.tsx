import {
  Anchor,
  Badge,
  Button,
  Card,
  Container,
  Divider,
  Grid,
  Group,
  Paper,
  rem,
  Stack,
  Text,
  Title,
  useMantineTheme,
  Image,
  useMantineColorScheme,
} from "@mantine/core";
import {
  Rocket,
  User,
  Settings,
  MessageCircleCode,
  LayoutList,
  ListCheck,
  Bolt,
} from "lucide-react";
import { useNavigate } from "react-router-dom";

const features = [
  {
    icon: User,
    title: "Organizations",
    description:
      "Create and manage your own organizations. Invite team members and collaborate efficiently.",
  },
  {
    icon: LayoutList,
    title: "Projects & Tasks",
    description:
      "Manage multiple projects. Create detailed tasks with deadlines and dependencies.",
  },
  {
    icon: ListCheck,
    title: "Custom Statuses",
    description:
      "Define custom statuses and task workflows that match your team’s process.",
  },
  {
    icon: MessageCircleCode,
    title: "Built-in Chat AI",
    description:
      "Use ChatAI to quickly describe your ideas and generate tasks or project plans instantly.",
  },
  {
    icon: Settings,
    title: "Full Customization",
    description:
      "Control roles, access, structure, and more with flexible configuration options.",
  },
  {
    icon: Rocket,
    title: "Easy Onboarding",
    description:
      "Sign in with Microsoft and get started in seconds — no complex setup needed.",
  },
];

const Home = () => {
  const navigate = useNavigate();
  const { colorScheme, setColorScheme } = useMantineColorScheme();
  const isDark = colorScheme === 'dark';

  return (
    <>
      {/* Header */}
      <header
        style={{
          padding: rem(16),
          backgroundColor: isDark ? "dark" : "white",
          position: "sticky",
          top: 0,
          zIndex: 1000,
        }}
      >
        <Container
          size="lg"
          style={{
            display: "flex",
            justifyContent: "space-between",
            alignItems: "center",
          }}
        >
          <Group gap="xs">
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
          </Group>

          <Group gap="lg">
            <Anchor href="#features" c="dimmed">
              Features
            </Anchor>
            <Anchor href="#benefits" c="dimmed">
              Why Us
            </Anchor>
            <Anchor href="#audience" c="dimmed">
              For Whom
            </Anchor>
            <Anchor href="#support" c="dimmed">
              Support
            </Anchor>
            <Button variant="light" onClick={() => navigate("/me")}>Login</Button>
          </Group>
        </Container>
      </header>

      {/* Main Section */}
      <Container size="lg" py="xl">
        <Stack align="center" gap="xs" ta="center">
          <Badge color="yellow" variant="filled">Beta Version</Badge>
          <Title order={1} fw={900} size="3rem">
            Smart Task Tracking
          </Title>
          <Text size="lg" c="dimmed" maw={700}>
            Manage tasks, projects, and organizations with a flexible system and
            built-in AI assistant. Create, collaborate, and move faster.
          </Text>
          <Button size="lg" radius="xl" onClick={() => navigate("/me")}>Get Started</Button>
        </Stack>

        {/* Screenshots */}
        <Stack mt="xl" align="center">
          <Title order={2} ta="center">How it looks</Title>
          <Text c="dimmed" ta="center">Take a quick peek at the interface (demo images):</Text>
          <Group grow mt="md">
            <Image src="/demo-screenshot-1.png" alt="Kanban Board" height={200} radius="md" />
            <Image src="/demo-screenshot-2.png" alt="AI Chat Assistant" height={200} radius="md" />
          </Group>
        </Stack>

        <Divider my="xl" />

        {/* Features Section */}
        <Title order={2} ta="center" mb="md" id="features">
          What You'll Get
        </Title>

        <Grid gutter="xl">
          {features.map((feature) => (
            <Grid.Col span={{ base: 12, sm: 6, md: 4 }} key={feature.title}>
              <Card shadow="sm" radius="md" p="lg" withBorder h="100%">
                <Group align="flex-start" wrap="nowrap">
                  <feature.icon size={28} color={isDark ? "white" : "black"} />
                  <div>
                    <Title order={4} size="h5" mt="xs">
                      {feature.title}
                    </Title>
                    <Text size="sm" mt="xs" c="dimmed">
                      {feature.description}
                    </Text>
                  </div>
                </Group>
              </Card>
            </Grid.Col>
          ))}
        </Grid>

        {/* Benefits */}
        <Stack align="center" mt="xl" id="benefits">
          <Title order={2}>Why TaskType?</Title>
          <Text maw={700} ta="center" c="dimmed">
            Designed to scale with your needs. Whether you're managing a side hustle, freelancing, or leading an enterprise team – TaskType adapts.
          </Text>
        </Stack>

        {/* Audience */}
        <Stack align="center" mt="xl" id="audience">
          <Title order={2}>Who is it for?</Title>
          <Text maw={700} ta="center" c="dimmed">
            Startups, freelancers, mid-size businesses, large teams, and anyone needing clarity in task management.
          </Text>
        </Stack>

        {/* Support Section */}
        <Stack align="center" mt="xl" id="support">
          <Title order={2}>Support the project ❤️</Title>
          <Text maw={700} ta="center" c="dimmed">
            TaskType is in beta. If you love it, consider supporting us with crypto.
          </Text>
          <Button variant="subtle" color="gray">Donate via Crypto</Button>
        </Stack>

        {/* Final CTA */}
        <Paper mt="xl" p="xl" radius="md" shadow="sm" ta="center">
          <Title order={3}>Ready to get started?</Title>
          <Text size="md" c="dimmed" mt="xs">
            Sign in and start building your first project in seconds.
          </Text>
          <Button size="md" radius="xl" mt="md" onClick={() => navigate("/signin-oidc")}>Sign Up Now</Button>
        </Paper>
      </Container>
    </>
  );
};

export default Home;