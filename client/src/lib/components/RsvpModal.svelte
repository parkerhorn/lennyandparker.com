<script>
  import { Button } from "$lib/components/ui/button";
  import * as Dialog from "$lib/components/ui/dialog";
  import { Input } from "$lib/components/ui/input";
  import { Label } from "$lib/components/ui/label";
  import * as RadioGroup from "$lib/components/ui/radio-group";
  import { rsvpApi } from '$lib/config/api.js';
  import { registryUrl } from '$lib/config/weddingData.js';

  let { open = $bindable(false) } = $props();

  // Pre-warm API when modal opens
  $effect(() => {
    if (open) {
      fetch('https://wedding-api-dev.azurewebsites.net/health').catch(() => {});
    }
  });

  // Reset state when modal closes
  $effect(() => {
    if (!open) {
      setTimeout(() => {
        resetState();
      }, 300);
    }
  });

  // State Management
  let currentStep = $state(1);
  let isSubmitted = $state(false);
  let isSubmitting = $state(false);
  let errorMessage = $state('');
  let searchName = $state('');
  let foundGuest = $state(null);
  let guestData = $state(null);
  let isRsvpingForBoth = $state(false);
  
  let mainGuestForm = $state({
    fullName: '',
    email: '',
    isAttending: '',
    pronouns: '',
    dietaryRestrictions: '',
    accessibilityRequirements: '',
    note: ''
  });

  let plusOneForm = $state({
    fullName: '',
    email: '',
    isAttending: '',
    pronouns: '',
    dietaryRestrictions: '',
    accessibilityRequirements: '',
    note: ''
  });

  // Reset all state
  function resetState() {
    currentStep = 1;
    isSubmitted = false;
    isSubmitting = false;
    errorMessage = '';
    searchName = '';
    foundGuest = null;
    guestData = null;
    isRsvpingForBoth = false;
    
    mainGuestForm = {
      fullName: '',
      email: '',
      isAttending: '',
      pronouns: '',
      dietaryRestrictions: '',
      accessibilityRequirements: '',
      note: ''
    };

    plusOneForm = {
      fullName: '',
      email: '',
      isAttending: '',
      pronouns: '',
      dietaryRestrictions: '',
      accessibilityRequirements: '',
      note: ''
    };
  }

  // Navigation Functions
  function nextStep() {
    currentStep++;
  }

  function prevStep() {
    currentStep--;
  }

  // Name parsing utility
  function parseFullName(fullName) {
    const parts = fullName.trim().split(' ');
    return {
      firstName: parts[0] || '',
      lastName: parts.slice(1).join(' ') || ''
    };
  }

  // Search for guest
  async function searchForGuest() {
    if (!searchName.trim()) {
      errorMessage = 'Please enter a name to search';
      return;
    }

    isSubmitting = true;
    errorMessage = '';

    try {
      const { firstName, lastName } = parseFullName(searchName);
      const rsvps = await rsvpApi.searchGuest(firstName, lastName);
      
      guestData = rsvps;
      foundGuest = rsvps[0]; // First RSVP is always the matched person
      
      // Pre-fill main guest form
      mainGuestForm.fullName = `${foundGuest.firstName} ${foundGuest.lastName}`;
      mainGuestForm.email = foundGuest.email;
      
      // If there's a second RSVP, it's the plus-one
      if (rsvps.length > 1) {
        const plusOne = rsvps[1];
        plusOneForm.fullName = `${plusOne.firstName} ${plusOne.lastName}`;
        plusOneForm.email = plusOne.email;
      }
      
      nextStep();
    } catch (error) {
      errorMessage = 'Guest not found. Please check the name and try again, or continue as a new guest.';
      
      // Allow continuing as new guest
      mainGuestForm.fullName = searchName;
      foundGuest = null;
      guestData = null;
    } finally {
      isSubmitting = false;
    }
  }

  // Continue as new guest
  function continueAsNewGuest() {
    mainGuestForm.fullName = searchName;
    foundGuest = null;
    guestData = null;
    errorMessage = '';
    nextStep();
  }

  // Step validation
  function validateCurrentStep() {
    errorMessage = '';
    
    switch (currentStep) {
      case 1:
        if (!searchName.trim()) {
          errorMessage = 'Please enter your name';
          return false;
        }
        break;
      case 2:
        // Guest selection step - no validation needed
        break;
      case 3:
        if (!mainGuestForm.isAttending) {
          errorMessage = 'Please select whether you will attend';
          return false;
        }
        if (isRsvpingForBoth && !plusOneForm.isAttending) {
          errorMessage = 'Please select whether your plus-one will attend';
          return false;
        }
        break;
      case 4:
        if (mainGuestForm.isAttending === "true" && !mainGuestForm.email.trim()) {
          errorMessage = 'Please enter your email address';
          return false;
        }
        if (isRsvpingForBoth && plusOneForm.isAttending === "true" && !plusOneForm.email.trim()) {
          errorMessage = 'Please enter your plus-one\'s email address';
          return false;
        }
        break;
    }
    
    return true;
  }

  // Navigate to next step with validation
  function goToNextStep() {
    if (validateCurrentStep()) {
      // Skip details for declining guests
      if (currentStep === 3 && mainGuestForm.isAttending === "false" && (!isRsvpingForBoth || plusOneForm.isAttending === "false")) {
        submitRsvp();
      } else {
        nextStep();
      }
    }
  }

  // Handle guest type selection
  function selectGuestType(rsvpForBoth) {
    isRsvpingForBoth = rsvpForBoth;
    nextStep();
  }

  // Submit RSVP
  async function submitRsvp() {
    if (!validateCurrentStep()) {
      return;
    }

    isSubmitting = true;
    errorMessage = '';
    
    try {
      const rsvps = [];
      
      // Main guest RSVP
      const { firstName: mainFirstName, lastName: mainLastName } = parseFullName(mainGuestForm.fullName);
      const mainRsvp = {
        FirstName: mainFirstName,
        LastName: mainLastName,
        Email: mainGuestForm.email || 'noemail@declined.com',
        IsAttending: mainGuestForm.isAttending === "true",
        DietaryRestrictions: mainGuestForm.dietaryRestrictions || null,
        AccessibilityRequirements: mainGuestForm.accessibilityRequirements || null,
        Pronouns: mainGuestForm.pronouns || null,
        Note: mainGuestForm.note || null,

      };
      rsvps.push(mainRsvp);

      // Plus-one RSVP if applicable
      if (isRsvpingForBoth && plusOneForm.fullName.trim()) {
        const { firstName: plusFirstName, lastName: plusLastName } = parseFullName(plusOneForm.fullName);
        const plusOneRsvp = {
          FirstName: plusFirstName,
          LastName: plusLastName,
          Email: plusOneForm.email || 'noemail@declined.com',
          IsAttending: plusOneForm.isAttending === "true",
          DietaryRestrictions: plusOneForm.dietaryRestrictions || null,
          AccessibilityRequirements: plusOneForm.accessibilityRequirements || null,
          Pronouns: plusOneForm.pronouns || null,
          Note: plusOneForm.note || null,
  
        };
        rsvps.push(plusOneRsvp);
      }

      await rsvpApi.submit(rsvps);

      isSubmitted = true;
      currentStep = 5; // Success step
    } catch (error) {
      console.error('Failed to submit RSVP:', error);
      errorMessage = error.message || 'Failed to submit RSVP. Please try again.';
    } finally {
      isSubmitting = false;
    }
  }

  function closeModal() {
    open = false;
  }

  // Get current guest name for display
  function getCurrentGuestName() {
    return mainGuestForm.fullName || searchName || 'Guest';
  }

  // Get plus-one name for display
  function getPlusOneName() {
    return plusOneForm.fullName || 'Plus-One';
  }
</script>

{#snippet errorDisplay()}
  {#if errorMessage}
    <div class="p-3 bg-destructive/10 border border-destructive/20 rounded-md text-destructive text-sm" role="alert">
      {errorMessage}
    </div>
  {/if}
{/snippet}

{#snippet rsvpForm(formData, title)}
  <div class="grid gap-[var(--spacing-element)]">
    <h4 class="font-medium text-lg">{title}</h4>
    
    <!-- Attendance Selection -->
    <div class="grid gap-2">
      <Label class="font-medium">Will you attend?</Label>
      <RadioGroup.Root bind:value={formData.isAttending} class="grid gap-3">
        <div class="flex items-center space-x-3 p-3 border rounded-md hover:bg-green-50 border-green-200 hover:border-green-300 transition-colors">
          <RadioGroup.Item value="true" id="accept-{title}" class="text-green-600 border-green-600" />
          <Label for="accept-{title}" class="cursor-pointer flex-1 text-green-800">Joyfully Accepts</Label>
        </div>
        <div class="flex items-center space-x-3 p-3 border rounded-md hover:bg-red-50 border-red-200 hover:border-red-300 transition-colors">
          <RadioGroup.Item value="false" id="decline-{title}" class="text-red-500 border-red-500" />
          <Label for="decline-{title}" class="cursor-pointer flex-1 text-red-700">Regretfully Declines</Label>
        </div>
      </RadioGroup.Root>
    </div>

    {#if formData.isAttending === "true"}
      <!-- Email -->
      <div class="grid gap-2">
        <Label for="email-{title}" class="font-medium">Email Address</Label>
        <Input 
          id="email-{title}" 
          type="email" 
          placeholder="your.email@example.com" 
          bind:value={formData.email}
        />
      </div>

      <!-- Pronouns -->
      <fieldset>
        <legend class="mb-2 block font-medium">Pronouns</legend>
        <RadioGroup.Root bind:value={formData.pronouns} class="flex flex-wrap gap-4">
          <div class="flex items-center space-x-2">
            <RadioGroup.Item value="she/her" id="she-{title}" class="text-primary border-input" />
            <Label for="she-{title}">She/her</Label>
          </div>
          <div class="flex items-center space-x-2">
            <RadioGroup.Item value="he/him" id="he-{title}" class="text-primary border-input" />
            <Label for="he-{title}">He/him</Label>
          </div>
          <div class="flex items-center space-x-2">
            <RadioGroup.Item value="they/them" id="they-{title}" class="text-primary border-input" />
            <Label for="they-{title}">They/them</Label>
          </div>
        </RadioGroup.Root>
      </fieldset>

      <!-- Dietary Restrictions -->
      <div class="grid gap-2">
        <Label for="dietary-{title}" class="font-medium">Dietary restrictions or allergies?</Label>
        <Input
          id="dietary-{title}"
          placeholder="e.g., Peanut allergy, gluten-free..."
          bind:value={formData.dietaryRestrictions}
        />
      </div>

      <!-- Accessibility -->
      <div class="grid gap-2">
        <Label for="accessibility-{title}" class="font-medium">Accessibility accommodations needed?</Label>
        <Input 
          id="accessibility-{title}" 
          placeholder="e.g., wheelchair access, hearing assistance..."
          bind:value={formData.accessibilityRequirements}
        />
      </div>

      <!-- Note -->
      <div class="grid gap-2">
        <Label for="note-{title}" class="font-medium">Favorite love song? (Optional)</Label>
        <Input 
          id="note-{title}" 
          placeholder="Artist - Song Title"
          bind:value={formData.note}
        />
      </div>
    {/if}
  </div>
{/snippet}

<Dialog.Root bind:open>
  <Dialog.Trigger>
    <Button variant="wedding" size="lg" class="font-sans text-lg px-8 py-3">
      RSVP Now
    </Button>
  </Dialog.Trigger>
  <Dialog.Content class="container-query bg-card border max-h-[90vh] overflow-y-auto" portalProps={{}}>
    <Dialog.Header>
      <Dialog.Title class="text-card-foreground font-serif text-center">
        {#if currentStep === 1}
          What's Your Name?
        {:else if currentStep === 2}
          {#if foundGuest && guestData?.length > 1}
            Hi {getCurrentGuestName()}! 
          {:else if foundGuest}
            Hi {getCurrentGuestName()}!
          {:else}
            Guest Not Found
          {/if}
        {:else if currentStep === 3}
          Will You Attend?
        {:else if currentStep === 4}
          RSVP Details
        {:else if currentStep === 5}
          <div class="text-2xl text-primary" aria-hidden="true">⋅˚₊‧ ୨୧ ‧₊˚ ⋅</div>
        {/if}
      </Dialog.Title>
    </Dialog.Header>

    <div class="py-4" role="main" aria-live="polite">
      {#if currentStep === 1}
        <!-- Name Search Step -->
        <div class="grid gap-[var(--spacing-element)]">
          <div class="grid gap-2">
            <Label for="guestName" class="font-medium">Enter your name to find your invitation</Label>
            <Input 
              id="guestName" 
              placeholder="e.g., John Smith" 
              type="text" 
              bind:value={searchName}
              onkeydown={(e) => e.key === 'Enter' && searchForGuest()}
            />
          </div>
          <Button onclick={searchForGuest} variant="wedding" class="font-sans" disabled={isSubmitting}>
            {isSubmitting ? 'Searching...' : 'Find My Invitation'}
          </Button>
          {@render errorDisplay()}
        </div>

      {:else if currentStep === 2}
        <!-- Guest Selection Step -->
        <div class="grid gap-[var(--spacing-element)]">
          {#if foundGuest && guestData?.length > 1}
            <div class="text-center mb-4">
              <p class="mb-4">We found your invitation! You can RSVP for yourself, or for both you and your plus-one.</p>
              <div class="grid gap-3">
                <Button onclick={() => selectGuestType(false)} variant="outline" class="font-sans p-4 h-auto">
                  <div class="text-left">
                    <div class="font-medium">Just Me</div>
                    <div class="text-sm text-muted-foreground">RSVP for {getCurrentGuestName()} only</div>
                  </div>
                </Button>
                <Button onclick={() => selectGuestType(true)} variant="outline" class="font-sans p-4 h-auto">
                  <div class="text-left">
                    <div class="font-medium">Both of Us</div>
                    <div class="text-sm text-muted-foreground">RSVP for {getCurrentGuestName()} and plus-one</div>
                  </div>
                </Button>
              </div>
            </div>
          {:else if foundGuest}
            <div class="text-center mb-4">
              <p class="mb-4">We found your invitation! Let's get your RSVP submitted.</p>
              <Button onclick={() => selectGuestType(false)} variant="wedding" class="font-sans">
                Continue with RSVP
              </Button>
            </div>
          {:else}
            <div class="text-center mb-4">
              <p class="mb-4">We couldn't find your invitation, but you can still RSVP as a new guest.</p>
              <Button onclick={continueAsNewGuest} variant="wedding" class="font-sans">
                Continue as New Guest
              </Button>
            </div>
          {/if}
          
          <div class="flex justify-center">
            <Button variant="ghost" onclick={prevStep} class="font-sans">Back</Button>
          </div>
        </div>

      {:else if currentStep === 3}
        <!-- Attendance Selection Step -->
        <div class="grid gap-[var(--spacing-element)]">
          <!-- Main Guest Attendance -->
          <div class="grid gap-2">
            <Label class="font-medium text-lg">{getCurrentGuestName()}, will you attend?</Label>
            <RadioGroup.Root bind:value={mainGuestForm.isAttending} class="grid gap-3">
              <div class="flex items-center space-x-3 p-4 border rounded-md hover:bg-green-50 border-green-200 hover:border-green-300 transition-colors">
                <RadioGroup.Item value="true" id="main-accept" class="text-green-600 border-green-600" />
                <Label for="main-accept" class="text-lg cursor-pointer flex-1 text-green-800">Joyfully Accepts</Label>
              </div>
              <div class="flex items-center space-x-3 p-4 border rounded-md hover:bg-red-50 border-red-200 hover:border-red-300 transition-colors">
                <RadioGroup.Item value="false" id="main-decline" class="text-red-500 border-red-500" />
                <Label for="main-decline" class="text-lg cursor-pointer flex-1 text-red-700">Regretfully Declines</Label>
              </div>
            </RadioGroup.Root>
          </div>

          <!-- Plus-One Attendance (if applicable) -->
          {#if isRsvpingForBoth}
            <div class="grid gap-2 mt-6 pt-6 border-t">
              <div class="grid gap-2 mb-4">
                <Label for="plusOneName" class="font-medium">Plus-One's Name</Label>
                <Input 
                  id="plusOneName" 
                  placeholder="e.g., Jane Doe" 
                  bind:value={plusOneForm.fullName}
                />
              </div>
              
              <Label class="font-medium text-lg">Will your plus-one attend?</Label>
              <RadioGroup.Root bind:value={plusOneForm.isAttending} class="grid gap-3">
                <div class="flex items-center space-x-3 p-4 border rounded-md hover:bg-green-50 border-green-200 hover:border-green-300 transition-colors">
                  <RadioGroup.Item value="true" id="plus-accept" class="text-green-600 border-green-600" />
                  <Label for="plus-accept" class="text-lg cursor-pointer flex-1 text-green-800">Joyfully Accepts</Label>
                </div>
                <div class="flex items-center space-x-3 p-4 border rounded-md hover:bg-red-50 border-red-200 hover:border-red-300 transition-colors">
                  <RadioGroup.Item value="false" id="plus-decline" class="text-red-500 border-red-500" />
                  <Label for="plus-decline" class="text-lg cursor-pointer flex-1 text-red-700">Regretfully Declines</Label>
                </div>
              </RadioGroup.Root>
            </div>
          {/if}

          <div class="flex justify-between mt-6">
            <Button variant="outline" onclick={prevStep} class="font-sans">Back</Button>
            <Button onclick={goToNextStep} variant="wedding" class="font-sans">Continue</Button>
          </div>
          {@render errorDisplay()}
        </div>

      {:else if currentStep === 4}
        <!-- Details Step -->
        <div class="grid gap-6">
          <!-- Main Guest Details -->
          {#if mainGuestForm.isAttending === "true"}
            {@render rsvpForm(mainGuestForm, getCurrentGuestName())}
          {/if}

          <!-- Plus-One Details -->
          {#if isRsvpingForBoth && plusOneForm.isAttending === "true"}
            <div class="border-t pt-6">
              {@render rsvpForm(plusOneForm, getPlusOneName())}
            </div>
          {/if}

          <div class="flex justify-between mt-6">
            <Button variant="outline" onclick={prevStep} class="font-sans">Back</Button>
            <Button onclick={submitRsvp} variant="wedding" class="font-sans" disabled={isSubmitting}>
              {isSubmitting ? 'Submitting...' : 'Submit RSVP'}
            </Button>
          </div>
          {@render errorDisplay()}
        </div>

      {:else if currentStep === 5}
        <!-- Success Step -->
        <div class="grid gap-[var(--spacing-element)] text-center">
          {#if mainGuestForm.isAttending === "true" || (isRsvpingForBoth && plusOneForm.isAttending === "true")}
            <div>
              <h3 class="text-lg font-medium mb-4">RSVP Submitted!</h3>
              <p>Thank you for responding. We can't wait to celebrate with you!</p>
            </div>
          {:else}
            <div>
              <h3 class="text-lg font-medium mb-4">We'll Miss You!</h3>
              <p>Thank you for letting us know. We'll miss having you there, but we understand!</p>
            </div>
          {/if}
          <Button 
            href={registryUrl} 
            variant="wedding" 
            size="sm"
            class="font-sans" 
            target="_blank" 
            rel="noopener noreferrer"
            onclick={closeModal}
          >
            VIEW REGISTRY
          </Button>
        </div>
      {/if}
    </div>
  </Dialog.Content>
</Dialog.Root>