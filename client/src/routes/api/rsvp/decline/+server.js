export const POST = async ({ request }) => {
  try {
    const body = await request.json();
    if (!body || typeof body.partyId !== 'string' || typeof body.responses !== 'object') {
      return new Response(JSON.stringify({ error: 'Invalid payload' }), { status: 400 });
    }
    return new Response(JSON.stringify({ ok: true, declined: true }), {
      status: 200,
      headers: { 'Content-Type': 'application/json' }
    });
  } catch (e) {
    return new Response(JSON.stringify({ error: 'Bad request' }), { status: 400 });
  }
};