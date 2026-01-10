interface ErrorEntry {
  occurredAt: string,
  message: string
}

interface ErrorJournal {
  errors: ErrorEntry[]
}

export type {
  ErrorJournal,
  ErrorEntry
}