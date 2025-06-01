import { NavLink, useLocation } from 'react-router-dom';
import { ActionIcon, Burger, Container, Stack, Text, useMantineColorScheme } from '@mantine/core';
import { useState } from 'react';
import classes from '../styles/Sidebar.module.css';
import {
  IconHome,
  IconBrandTrello,
  IconPackages,
  IconUserCircle,
  IconSun,
  IconMoon,
} from '@tabler/icons-react';

const data = [
  { link: '/#', label: 'Home', icon: IconHome },
  { link: '/kanban', label: 'Kanban', icon: IconBrandTrello },
  { link: '/backlog', label: 'Backlog', icon: IconPackages },
  { link: '/profile', label: 'Profile', icon: IconUserCircle },
];

// export function Sidebar({ opened, toggle }: { opened: boolean; toggle: () => void }) {
export function Sidebar() {
  const location = useLocation();
  const { colorScheme, setColorScheme } = useMantineColorScheme();

  const links = data.map((item) => (
    <NavLink
      to={item.link}
      key={item.label}
      className={({ isActive }) =>
        `${classes.link} ${isActive ? classes.active : ''}`
      }
    >
      <item.icon className={classes.linkIcon} stroke={1.5} />
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
          {colorScheme === 'dark' ? <IconSun size={18} /> : <IconMoon size={18} />}
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