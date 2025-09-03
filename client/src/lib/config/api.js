// API configuration for direct backend calls
import { browser } from '$app/environment';

// Get API base URL from environment or use default
const getApiBaseUrl = () => {
  if (browser) {
    // Always use the deployed Azure API URL
    return 'https://wedding-api-dev.azurewebsites.net';
  }
  
  // Server-side rendering fallback
  return process.env.API_BASE_URL || 'https://wedding-api-dev.azurewebsites.net';
};

export const API_BASE_URL = getApiBaseUrl();

// API helper functions with proper error handling
export async function apiCall(endpoint, options = {}) {
  try {
    const response = await fetch(`${API_BASE_URL}${endpoint}`, {
      headers: {
        'Content-Type': 'application/json',
        ...options.headers
      },
      ...options
    });

    if (!response.ok) {
      // Don't expose detailed backend errors to users
      const isClientError = response.status >= 400 && response.status < 500;
      throw new Error(
        isClientError 
          ? 'Please check your information and try again.'
          : 'Service temporarily unavailable. Please try again later.'
      );
    }

    return await response.json();
  } catch (error) {
    // Log for debugging but show user-friendly message
    console.error(`API call failed for ${endpoint}:`, error);
    
    if (error.name === 'TypeError' && error.message.includes('fetch')) {
      throw new Error('Unable to connect. Please check your internet connection.');
    }
    
    throw error;
  }
}

// RSVP-specific API functions
export const rsvpApi = {
  // Submit new RSVP(s)
  async submit(rsvps) {
    return apiCall('/rsvp', {
      method: 'POST',
      body: JSON.stringify(Array.isArray(rsvps) ? rsvps : [rsvps])
    });
  },

  // Get all RSVPs (for checking existing responses)
  async getAll() {
    return apiCall('/rsvp');
  },

  // Get specific RSVP by ID
  async getById(id) {
    return apiCall(`/rsvp/${id}`);
  },

  // Update existing RSVP
  async update(id, rsvp) {
    return apiCall(`/rsvp/${id}`, {
      method: 'PUT',
      body: JSON.stringify(rsvp)
    });
  },

  // Search for guest by name (fuzzy matching)
  async searchGuest(firstName, lastName) {
    const params = new URLSearchParams();
    if (firstName) params.append('firstName', firstName);
    if (lastName) params.append('lastName', lastName);
    
    return apiCall(`/rsvp/search?${params.toString()}`);
  },




};