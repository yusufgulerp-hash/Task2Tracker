import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import * as projectsApi from '../api/projects'
import type { ProjectListItem } from '../api/types'
import { getApiErrorMessage } from '../api/client'
import { useDebouncedValue } from '../hooks/useDebouncedValue'

export function ProjectsPage() {
  const [projects, setProjects] = useState<ProjectListItem[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  const [searchText, setSearchText] = useState('')
  const debouncedSearch = useDebouncedValue(searchText, 300)

  useEffect(() => {
    const trimmed = debouncedSearch.trim()
    const request = trimmed
      ? projectsApi.searchProjects(trimmed)
      : projectsApi.getProjects()

    request
      .then(setProjects)
      .catch((err) => setError(getApiErrorMessage(err)))
      .finally(() => setIsLoading(false))
  }, [debouncedSearch])

  return (
    <div>
      <div className="mb-6">
        <h1 className="font-display text-2xl font-semibold text-ink">Projeler</h1>
        <p className="mt-1 text-sm text-ink-muted">
          Üyesi olduğun projeler burada listelenir.
        </p>
      </div>

      <div className="mb-6">
        <input
          value={searchText}
          onChange={(e) => setSearchText(e.target.value)}
          placeholder="Proje adında ara..."
          className="w-full max-w-sm rounded-md border border-border bg-surface px-3 py-2 text-sm text-ink outline-none focus:border-branch focus:ring-1 focus:ring-branch sm:max-w-xs"
        />
      </div>

      {error && (
        <p className="mb-4 rounded-md bg-red-soft px-3 py-2 text-sm text-red">{error}</p>
      )}

      {isLoading ? (
        <p className="text-sm text-ink-muted">Yükleniyor...</p>
      ) : projects.length === 0 ? (
        <div className="rounded-xl border border-dashed border-border px-6 py-12 text-center">
          <p className="text-sm text-ink-muted">
            {searchText.trim()
              ? 'Aramanla eşleşen bir proje bulunamadı.'
              : 'Henüz üyesi olduğun bir proje yok.'}
          </p>
        </div>
      ) : (
        <ul className="grid grid-cols-1 gap-3 sm:grid-cols-2">
          {projects.map((project) => (
            <li key={project.id}>
              <Link
                to={`/projects/${project.id}`}
                className="group flex items-center gap-3 rounded-xl border border-border bg-surface px-5 py-4 transition-colors hover:border-branch"
              >
                <span className="h-2 w-2 shrink-0 rounded-full bg-branch" />
                <span className="font-medium text-ink group-hover:text-branch">
                  {project.name}
                </span>
              </Link>
            </li>
          ))}
        </ul>
      )}
    </div>
  )
}
