export function load() {
  function calculateDaysRemaining() {
    const weddingDate = new Date("2025-11-08");
    const today = new Date();

    // Set time to start of day for accurate day calculation
    today.setHours(0, 0, 0, 0);
    weddingDate.setHours(0, 0, 0, 0);

    const timeDiff = weddingDate.getTime() - today.getTime();
    const daysDiff = Math.ceil(timeDiff / (1000 * 3600 * 24));

    return Math.max(0, daysDiff);
  }

  return {
    daysRemaining: calculateDaysRemaining()
  };
}