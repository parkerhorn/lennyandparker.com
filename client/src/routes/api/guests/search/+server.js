import { demoParties } from '$lib/config/mockData.js';

export const POST = async ({ request }) => {
  try {
    const { query } = await request.json();
    const q = (query ?? '').trim().toLowerCase();

    if (!q) {
      return new Response(JSON.stringify({ error: 'Missing query' }), {
        status: 400
      });
    }

    const match = demoParties.find((party) =>
      party.guests.some((g) => g.name.toLowerCase().includes(q))
    );

    if (!match) {
      return new Response(JSON.stringify({ error: 'Not found' }), {
        status: 404
      });
    }

    return new Response(JSON.stringify(match), {
      status: 200,
      headers: { 'Content-Type': 'application/json' }
    });
  } catch (e) {
    return new Response(JSON.stringify({ error: 'Bad request' }), {
      status: 400
    });
  }
};