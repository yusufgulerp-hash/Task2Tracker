import { useEffect, useState } from 'react'

/** Bir değerin, kullanıcı yazmayı bıraktıktan `delayMs` sonra güncellenen halini döner. */
export function useDebouncedValue<T>(value: T, delayMs = 300): T {
  const [debounced, setDebounced] = useState(value)

  useEffect(() => {
    const timer = setTimeout(() => setDebounced(value), delayMs)
    return () => clearTimeout(timer)
  }, [value, delayMs])

  return debounced
}
