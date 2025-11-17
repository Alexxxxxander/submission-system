class ApiError extends Error {
  constructor(message, statusCode, details) {
    super(message);
    this.name = 'ApiError';
    this.statusCode = statusCode;
    this.details = details;
  }
}

async function handleResponse(response) {
  if (!response.ok) {
    let errorMessage = `HTTP error ${response.status}`;
    let errorDetails;

    try {
      const errorData = await response.json();
      errorMessage = errorData.error || errorMessage;
      errorDetails = errorData.details;
    } catch {
      errorMessage = await response.text() || errorMessage;
    }

    throw new ApiError(errorMessage, response.status, errorDetails);
  }

  return response.json();
}

async function request(url, options) {
  try {
    const response = await fetch(url, {
      ...options,
      headers: {
        'Content-Type': 'application/json',
        ...options?.headers,
      },
    });

    return await handleResponse(response);
  } catch (error) {
    if (error instanceof ApiError) {
      throw error;
    }
    throw new ApiError('Network error occurred');
  }
}

export const httpClient = {
  get: (url) => request(url, { method: 'GET' }),
  
  post: (url, data) =>
    request(url, {
      method: 'POST',
      body: JSON.stringify(data),
    }),
  
  put: (url, data) =>
    request(url, {
      method: 'PUT',
      body: JSON.stringify(data),
    }),
  
  delete: (url) => request(url, { method: 'DELETE' }),
};

export { ApiError };


