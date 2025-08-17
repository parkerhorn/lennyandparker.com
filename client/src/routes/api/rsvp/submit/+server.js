const API_BASE_URL = process.env.API_BASE_URL || 'http://localhost:5000';

export const POST = async ({ request }) => {
  try {
    const body = await request.json();

    // Transform frontend format to backend RSVP format
    const rsvps = body.responses?.map(response => ({
      firstName: response.firstName || '',
      lastName: response.lastName || '',
      email: response.email || '',
      isAttending: response.attending || false,
      dietaryRestrictions: response.dietaryRestrictions || null,
      accessibilityRequirements: response.accessibilityRequirements || null,
      pronouns: response.pronouns || null,
      note: response.note || null
    })) || [];

    if (rsvps.length === 0) {
      return new Response(JSON.stringify({ error: 'No RSVP responses provided' }), { status: 400 });
    }

    // Send to C# backend
    const response = await fetch(`${API_BASE_URL}/rsvp`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(rsvps)
    });

    if (!response.ok) {
      const errorData = await response.text();
      return new Response(JSON.stringify({ error: 'Failed to submit RSVP', details: errorData }), { 
        status: response.status 
      });
    }

    const result = await response.json();
    return new Response(JSON.stringify({ ok: true, data: result }), {
      status: 201,
      headers: { 'Content-Type': 'application/json' }
    });
  } catch (e) {
    console.error('RSVP submission error:', e);
    return new Response(JSON.stringify({ error: 'Internal server error' }), { status: 500 });
  }
};