import { NavLink, useInRouterContext, useLocation, useParams } from 'react-router-dom';
import {
  House, Kanban, Scroll, UserRoundPen, Sun, Moon, Building2, BotMessageSquare, Settings, Users2, LayoutDashboard,
} from 'lucide-react';
import classes from '../styles/Sidebar.module.css';
import { useSafeAuth } from '../hooks/useSafeAuth';
import { useMemo } from 'react';

export function Sidebar() {
  const auth = useSafeAuth();
  const inRouter = useInRouterContext();
  const location = useLocation();
  const routeParams = useParams();
  const orgId = routeParams.orgId || routeParams.id;
  const projectId = routeParams.projectId || routeParams.id;;

  const sidebarLinks = useMemo(() => {
    if (location.pathname.includes('/project/') && projectId) {
      return [
        { link: '/me', label: 'Home', icon: House },
        { link: `/project/${projectId}`, label: 'Project details', icon: LayoutDashboard },
        { link: `/project/${projectId}/kanban`, label: 'Kanban', icon: Kanban },
        { link: `/project/${projectId}/backlog`, label: 'Backlog', icon: Scroll },
        { link: `/project/${projectId}/chatai`, label: 'ChatAI', icon: BotMessageSquare },
        { link: `/project/${projectId}/settings`, label: 'Settings', icon: Settings },
      ];
    }

    if (location.pathname.startsWith('/org/') && !location.pathname.includes('/project/') && orgId) {
      return [
        { link: '/me', label: 'Home', icon: House },
        { link: `/org/${orgId}`, label: 'Dashboard', icon: LayoutDashboard },
        { link: `/org/${orgId}/settings`, label: 'Settings', icon: Settings },
      ];
    }

    return [
      { link: '/me', label: 'Home', icon: House },
      // { link: '/organizations', label: 'Organizations', icon: Building2 },
      { link: '/profile', label: 'Profile', icon: UserRoundPen },
    ];
  }, [location.pathname, orgId, projectId]);

  const links = sidebarLinks.map((item) => (
    <NavLink
      to={item.link}
      key={item.label}
      end={item.link === `/project/${projectId}` || item.link === `/org/${orgId}`}
      className={({ isActive }) =>
        `${classes.link} ${isActive ? classes.active : ''}`
      }
    >

      <item.icon className={classes.linkIcon} />
      <span>{item.label}</span>
    </NavLink>
  ));

  if (!inRouter) return null;
  if (!auth.isAuthenticated) return null;

  return (
    <nav className={classes.navbar}>
      <div className={classes.navbarMain}>{links}</div>
    </nav>
  );
}
