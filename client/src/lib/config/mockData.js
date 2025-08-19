// Mock data for development - replace with real API calls in production

export const demoParties = [
  {
    id: 'party-smith',
    name: 'The Smith Party',
    guests: [
      { id: 'guest-john-smith', name: 'John Smith' },
      { id: 'guest-sarah-smith', name: 'Sarah Smith' }
    ]
  },
  {
    id: 'party-jane',
    name: 'Jane Doe',
    guests: [{ id: 'guest-jane-doe', name: 'Jane Doe' }]
  }
];

export const defaultRsvpParty = {
  partyName: "The Smith Couple",
  members: [
    { id: 2, name: "John Smith", attending: false },
    { id: 3, name: "Sarah Smith", attending: false },
  ],
};