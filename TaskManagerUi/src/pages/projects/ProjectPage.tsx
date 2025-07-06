import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { Container, Title, Text, Paper, Grid, Card, Divider, Badge, Button, Group } from "@mantine/core";
import { PieChart, Pie, Cell, Tooltip, ResponsiveContainer } from "recharts";
import { useProjectApi } from "../../api/taskManagerApi";
import { useIdentityServerApi } from "../../api/IdentityServerApi";
import { LoaderMain } from "../../components/LoaderMain";
import type { Project, Task, Status, AccountDetails } from "../../components/Types";
import NotFoundPage from "../NotFoundPage";
import { useOrgLocalStorage } from "../../hooks/useOrgLocalStorage";

const COLORS = ['#4dabf7', '#51cf66', '#ff922b', '#ff6b6b', '#845ef7'];

export default function ProjectDashboard() {
  const params = useParams<{ id?: string }>();
  const id = params?.id;
  const navigate = useNavigate();
  const { getProjectWithTasksById } = useProjectApi();
  const [project, setProject] = useState<Project | null>(null);
  const [tasks, setTasks] = useState<Task[]>([]);
  const [statuses, setStatuses] = useState<Status[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchData = async () => {
      try {
        if (!id) return;
        const result = await getProjectWithTasksById(id);
        setProject(result.data.project);
        setTasks(result.data.tasks);
        setStatuses(result.data.project.statuses ?? []);
      } catch (error) {
        console.error("Error loading project dashboard:", error);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [id]);

  useEffect(() => {
    if (project?.organizationId) {
      localStorage.setItem('organizationId', project?.organizationId);
    }
  }, [project?.organizationId]);

  if (!id || loading || !project) return <LoaderMain />;
  if (!id || id === 'undefined') return <NotFoundPage />;

  const totalTasks = tasks.length;
  const completedTasks = tasks.filter(task => {
    const status = statuses.find(s => s.statusId === task.statusId);
    return status?.typeId === 4;
  }).length;

  const pieData = statuses.map((status) => {
    const count = tasks.filter(task => task.statusId === status.statusId).length;
    return {
      name: status.statusName,
      value: count
    };
  }).filter(d => d.value > 0);

  return (
    <Container fluid py="xl">
      <Paper withBorder shadow="sm" radius="md" p="lg" mb="xl">
        <Group justify="space-between" align="center" mb="md">
          <div>
            <Title order={2} mb="xs">{project.title} - Dashboard</Title>
            <Text c="dimmed">{project.description}</Text>
          </div>
          <Group>
            <Button variant="outline" onClick={() => navigate('/organizations')}>Change Organization</Button>
            <Button variant="light" onClick={() => navigate(`/org/${project.organizationId}`)}>Back to Project List</Button>
          </Group>
        </Group>
      </Paper>

      <Grid gutter="xl">
        <Grid.Col span={{ base: 12, sm: 6, md: 4 }}>
          <Card withBorder radius="md" shadow="sm" p="lg">
            <Title order={4}>Total Tasks</Title>
            <Divider my="sm" />
            <Badge size="lg" variant="light">{totalTasks}</Badge>
          </Card>
        </Grid.Col>

        <Grid.Col span={{ base: 12, sm: 6, md: 4 }}>
          <Card withBorder radius="md" shadow="sm" p="lg">
            <Title order={4}>Completed Tasks</Title>
            <Divider my="sm" />
            <Badge color="green" size="lg" variant="light">{completedTasks}</Badge>
          </Card>
        </Grid.Col>

        <Grid.Col span={{ base: 12, sm: 12, md: 6 }}>
          <Card withBorder radius="md" shadow="sm" p="lg">
            <Title order={4}>Task Distribution by Status</Title>
            <Divider my="sm" />
            <ResponsiveContainer width="100%" height={250}>
              <PieChart>
                <Pie
                  data={pieData}
                  dataKey="value"
                  nameKey="name"
                  cx="50%"
                  cy="50%"
                  outerRadius={80}
                  label
                >
                  {pieData.map((entry, index) => (
                    <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                  ))}
                </Pie>
                <Tooltip />
              </PieChart>
            </ResponsiveContainer>
          </Card>
        </Grid.Col>
      </Grid>
    </Container>
  );
}
