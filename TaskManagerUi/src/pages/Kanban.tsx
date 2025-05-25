import { useEffect } from 'react'
import { useOrganizationApi } from '../api/taskManagerApi'

const Kanban = () => {
  const { getOrganizationProjects } = useOrganizationApi()

  useEffect(() => {
    const fetchOrganizationProjects = async () => {
      try {
        const projects = await getOrganizationProjects()
        console.log('Fetched projects:', projects)
      } catch (error) {
        console.error('Error fetching projects:', error)
      }
    }

    fetchOrganizationProjects()
  }, [getOrganizationProjects])

  return (
    <div>
      <h1>Kanban Board</h1>
      {/* Kanban board implementation goes here */}
    </div>
  )
}

export default Kanban
