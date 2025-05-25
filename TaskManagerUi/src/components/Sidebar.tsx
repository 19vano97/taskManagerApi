import { NavLink } from 'react-router-dom'
import { Stack, Text } from '@mantine/core'
import { useState } from 'react';
import {
  Icon2fa,
  IconBellRinging,
  IconDatabaseImport,
  IconFingerprint,
  IconKey,
  IconLogout,
  IconReceipt2,
  IconSettings,
  IconSwitchHorizontal,
} from '@tabler/icons-react';
import { Code, Group } from '@mantine/core';
// import classes from '../styles/Sidebar.module.css';
import '../styles/Sidebar.css';

// const data = [
//   { link: '/kanban', label: 'Kanban', icon: IconBellRinging },
//   { link: '/backlog', label: 'Backlog', icon: IconReceipt2 },
//   { link: 'profile', label: 'Profile', icon: IconFingerprint },
//   { link: '', label: 'SSH Keys', icon: IconKey },
//   { link: '', label: 'Databases', icon: IconDatabaseImport },
//   { link: '', label: 'Authentication', icon: Icon2fa },
//   { link: '', label: 'Other Settings', icon: IconSettings },
// ];

// export function Sidebar() {
//   // <aside className="sidebar">
//   //   <div className="logo">TaskType</div>
//   //   <nav>
//   //     <NavLink to="/" end>Home</NavLink>
//   //     <NavLink to="/kanban">Kanban</NavLink>
//   //     <NavLink to="/backlog">Backlog</NavLink>
//   //     <NavLink to="/profile">Profile</NavLink>
//   //   </nav>
//   // </aside>
//   const [active, setActive] = useState('Billing');

//   const links = data.map((item) => (
//     <a
//       className={classes.link}
//       data-active={item.label === active || undefined}
//       href={item.link}
//       key={item.label}
//       onClick={(event) => {
//         event.preventDefault();
//         setActive(item.label);
//       }}
//     >
//       <item.icon className={classes.linkIcon} stroke={1.5} />
//       <span>{item.label}</span>
//     </a>
//   ));

//   return (
//     <nav className={classes.navbar}>
//       <div className={classes.navbarMain}>
//         <Group className={classes.header} justify="space-between">
//           <img src="../assets/logo.svg" alt="logo" />
//         </Group>
//         {links}
//       </div>

//       <div className={classes.footer}>
//         <a href="#" className={classes.link} onClick={(event) => event.preventDefault()}>
//           <IconSwitchHorizontal className={classes.linkIcon} stroke={1.5} />
//           <span>Change account</span>
//         </a>

//         <a href="#" className={classes.link} onClick={(event) => event.preventDefault()}>
//           <IconLogout className={classes.linkIcon} stroke={1.5} />
//           <span>Logout</span>
//         </a>
//       </div>
//     </nav>
//   );
// }

// export function Sidebar() {
//   return (
//     <aside className="sidebar">
//       <div className="logo">TaskType</div>
//       <nav>
//         <NavLink to="/" end>Home</NavLink>
//         <NavLink to="/kanban">Kanban</NavLink>
//         <NavLink to="/backlog">Backlog</NavLink>
//         <NavLink to="/profile">Profile</NavLink>
//       </nav>
//     </aside>
//   )
// }

// export function Sidebar() {
//   return (
//     <aside className="sidebar">
//       <div className="logo">TaskType</div>
//       <nav>
//         <NavLink to="/" end>Home</NavLink>
//         <NavLink to="/kanban">Kanban</NavLink>
//         <NavLink to="/backlog">Backlog</NavLink>
//         <NavLink to="/profile">Profile</NavLink>
//       </nav>
//     </aside>
//   )
// }
export const Sidebar = () => (
  <Stack>
    <NavLink to="/" end>Home</NavLink>
    <NavLink to="/kanban">Kanban</NavLink>
    <NavLink to="/backlog">Backlog</NavLink>
    <NavLink to="/profile">Profile</NavLink>
  </Stack>
)