import { NavLink, useInRouterContext, useLocation } from 'react-router-dom';
import { ActionIcon, Burger, Container, Stack, Text, useMantineColorScheme } from '@mantine/core';
import { useState } from 'react';
import classes from '../styles/Sidebar.module.css';
import { House, Kanban, Scroll, UserRoundPen, Sun, Moon } from 'lucide-react';

const data = [
  { link: '/#', label: 'Home', icon: House },
  { link: '/kanban', label: 'Kanban', icon: Kanban },
  { link: '/backlog', label: 'Backlog', icon: Scroll },
  { link: '/profile', label: 'Profile', icon: UserRoundPen },
];

// export function Sidebar({ opened, toggle }: { opened: boolean; toggle: () => void }) {
export function Sidebar() {
  const inRouter = useInRouterContext();
  const location = useLocation();
  const { colorScheme, setColorScheme } = useMantineColorScheme();

  if (!inRouter) return null;

  const links = data.map((item) => (
    <NavLink
      to={item.link}
      key={item.label}
      className={({ isActive }) =>
        `${classes.link} ${isActive ? classes.active : ''}`
      }
    >
      <item.icon className={classes.linkIcon} />
      <span>{item.label}</span>
    </NavLink>
  ));

  return (
   <nav className={classes.navbar}>
      <div className={classes.navbarMain}>{links}</div>
      <ActionIcon
          variant="default"
          onClick={() => setColorScheme(colorScheme === 'dark' ? 'light' : 'dark')}
          size="lg"
          aria-label="Toggle color scheme"
        >
          {colorScheme === 'dark' ? <Sun size={18} /> : <Moon size={18} />}
      </ActionIcon>

      {/* <div className={classes.footer}>
        <a href="#" className={classes.link} onClick={(e) => e.preventDefault()}>
          <IconSwitchHorizontal className={classes.linkIcon} stroke={1.5} />
          <span className={classes.linkLabel}>Change account</span>
        </a>

        <a href="#" className={classes.link} onClick={(e) => e.preventDefault()}>
          <IconLogout className={classes.linkIcon} stroke={1.5} />
          <span className={classes.linkLabel}>Logout</span>
        </a>
      </div> */}
    </nav>


  );
}


// export function Sidebar() {
//   const [active, setActive] = useState<string | null>(null);
//   const location = useLocation();

//   return (
//     <aside className="sidebar">
//       <Container>
//         <Stack >
//           <nav>
//             <NavLink
//               to="/"
//               end
//               className={({ isActive }) => (isActive ? 'active' : '')}
//               onClick={() => setActive('home')}
//             >
//               Home
//             </NavLink>
//             <NavLink
//               to="/kanban"
//               className={({ isActive }) => (isActive ? 'active' : '')}
//               onClick={() => setActive('kanban')}
//             >
//               Kanban
//             </NavLink>
//             <NavLink
//               to="/backlog"
//               className={({ isActive }) => (isActive ? 'active' : '')}
//               onClick={() => setActive('backlog')}
//             >
//               Backlog
//             </NavLink>
//             <NavLink
//               to="/profile"
//               className={({ isActive }) => (isActive ? 'active' : '')}
//               onClick={() => setActive('profile')}
//             >
//               Profile
//             </NavLink>
//           </nav>
//         </Stack>
//       </Container>
//     </aside>
//   );