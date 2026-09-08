export function getErrorMessage(err, fallback = 'Something went wrong. Please try again.') {
  const data = err.response?.data;
  if (!data) return fallback; // backend is not available, no response

  if (data.errors) {
    // Validation errors - merge into one message
    const messages = Object.values(data.errors).flat();
    if (messages.length) return messages.join(' ');
  }

  return data.detail || fallback;
}