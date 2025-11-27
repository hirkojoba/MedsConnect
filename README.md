# MedsConnect - Medication Tracker App

A comprehensive .NET MAUI cross-platform medication tracking application that helps users manage their medications, track adherence, and share progress with caregivers.

## Features

### User Management
- User registration and authentication
- Secure password hashing
- Profile management with personal information

### Medication Management
- Add, edit, and delete medications
- Track dosage, frequency, and scheduled times
- Set start and end dates for medications
- Mark medications as active or inactive
- Add notes and descriptions for each medication

### Medication Tracking
- Daily dashboard showing today's medication schedule
- Mark medications as taken, missed, or skipped
- View medication history by date
- Track adherence statistics (30-day rolling average)
- Automatic log generation for scheduled medications

### Reminders & Notifications
- Set customizable reminder times for each medication
- Local push notifications (15 minutes before scheduled time by default)
- Enable/disable reminders per medication
- Daily repeating notifications

### Caregiver Support
- Add caregivers by email
- Approve/reject caregiver requests
- Configure caregiver permissions:
  - View medications
  - View medication logs
  - Receive alerts
- View patients you care for
- Track multiple patients' medication adherence

## Screenshots

### Authentication & Onboarding

<p align="center">
  <img src="Screenshots/login.png" width="250" alt="Login Screen"/>
</p>

**User Authentication:** Secure login screen with email and password authentication.

<p align="center">
  <img src="Screenshots/notification permission.png" width="250" alt="Notification Permission"/>
</p>

**Notification Permissions:** On first use, the app requests notification permissions to enable medication reminders.

---

### Dashboard & Medication Tracking

<p align="center">
  <img src="Screenshots/dashboard.png" width="250" alt="Dashboard"/>
</p>

**Daily Dashboard:** Real-time view of today's medication schedule with color-coded status indicators. Displays adherence statistics including total medications, taken count, pending count, and 30-day rolling adherence percentage. Users can quickly mark medications as taken with a single tap.

<p align="center">
  <img src="Screenshots/medication list.png" width="250" alt="Medication List"/>
</p>

**Medication Management:** Complete list of all active medications with dosage information, frequency, medication description, notes and scheduled times. Supports filtering between active and inactive medications. Each medication card displays key information at a glance.

<p align="center">
  <img src="Screenshots/viewing inactive medications as well.png" width="250" alt="Active & Inactive Medications"/>
</p>

**Active & Inactive Medications:** Toggle view showing both active and inactive medications. Users can easily manage their complete medication history and reactivate previous medications as needed.

---

### Medication Management

<p align="center">
  <img src="Screenshots/adding medication pt 1.png" width="250" alt="Adding Medication - Part 1"/>
</p>

**Add Medication - Basic Info:** Comprehensive form for adding new medications with fields for name, description, dosage, unit, frequency, and detailed notes. Input validation ensures data integrity.

<p align="center">
  <img src="Screenshots/adding medication pt 2.png" width="250" alt="Adding Medication - Part 2"/>
</p>

**Add Medication - Schedule & Reminders:** Set multiple scheduled times per day using time pickers. Configure reminder settings including reminder offset time (default 15 minutes before). Supports start and end dates for temporary medications.

<p align="center">
  <img src="Screenshots/medication reminder notification.png" width="250" alt="Medication Reminder Notification"/>
</p>

**Local Push Notifications:** System-level notifications remind users to take medications at scheduled times. Notifications include medication name and dosage. Tapping opens the app directly to the medication details.

---

### History & Analytics

<p align="center">
  <img src="Screenshots/history.png" width="250" alt="Medication History"/>
</p>

**Medication History:** Calendar-based view of medication logs with date filtering. Shows detailed history of all medications taken, missed, or skipped with timestamps as well as logging who marked it, whether it be the patient or caregiver. Users can add notes to individual log entries for tracking side effects or other observations.

---

### Caregiver Features

<p align="center">
  <img src="Screenshots/caregiver request.png" width="250" alt="Patient Sending Caregiver Request"/>
</p>

**Patient - Send Caregiver Request:** Patients can add caregivers by email address and specify the relationship type (family member, healthcare provider, friend). Configurable permissions allow patients to control what data caregivers can access.

<p align="center">
  <img src="Screenshots/caregiver receiving request.png" width="250" alt="Caregiver Receiving Request"/>
</p>

**Caregiver - Receive Request:** Caregivers receive and can approve or reject patient requests. The approval system ensures both parties consent to the relationship before data sharing begins.

<p align="center">
  <img src="Screenshots/patient viewing caregiver list.png" width="250" alt="Patient's Caregiver List"/>
</p>

**Patient - Caregiver List:** Patients can view all their approved caregivers with relationship details and permission levels. Options to modify permissions or remove caregivers maintain patient control over their data.

<p align="center">
  <img src="Screenshots/caregiver patient list.png" width="250" alt="Caregiver's Patient List"/>
</p>

**Caregiver - Patient List:** Caregivers can manage multiple patients from a single interface. Shows list of all patients who have granted access, with quick navigation to each patient's medication data.

<p align="center">
  <img src="Screenshots/caregiver viewing patient's data.png" width="250" alt="Caregiver Viewing Patient Data"/>
</p>

**Caregiver - View Patient Dashboard:** Caregivers can view their patients' medication schedules and adherence statistics based on granted permissions. This enables remote monitoring and support for medication adherence. There is also a "back to my view" button at the top so that the caregiver can also view their own dashboard and etc if they track their own medication.

<p align="center">
  <img src="Screenshots/caregiver marks patient medication as taken.png" width="250" alt="Caregiver Marking Medication as Taken"/>
</p>

**Caregiver - Mark Medications Taken:** With appropriate permissions, caregivers can mark medications as taken on behalf of their patients and the caregiver's name will be marked under that medication's history. Useful for elderly patients, children, or those needing assistance with medication management.

## Download

**[Download Android APK (v1.0.0)](https://github.com/hirkojoba/MedsConnect/releases/latest)**

Install the APK on your Android device to try out MedsConnect!


### Technology Stack
- **.NET 9.0**
- **.NET MAUI** - Cross-platform framework
- **Entity Framework Core 9.0** - ORM for database management
- **SQLite** - Local database storage
- **CommunityToolkit.Maui 10.0** - Enhanced UI controls
- **CommunityToolkit.Mvvm** - MVVM helpers and commands
- **Plugin.LocalNotification** - Local push notifications


## Usage Guide

### Getting Started
1. Launch the app
2. Register a new account with your email
3. Log in with your credentials

### Adding Medications
1. Navigate to "Medications" tab
2. Tap "+ Add" button
3. Fill in medication details:
   - Name (required)
   - Dosage and unit (required)
   - Frequency
   - Scheduled times (add at least one)
   - Optional: description, notes, end date
4. Enable reminders if desired
5. Save

### Daily Tracking
1. Dashboard shows today's scheduled medications
2. Tap checkmark (✓) to mark as taken
3. View statistics: total, taken, pending, adherence %
4. Refresh to update the list

### Adding Caregivers
1. Navigate to "Caregivers" tab
2. Tap "+ Add Caregiver"
3. Enter caregiver's registered email
4. Select relationship type
5. Send request
6. Caregiver must approve the request

## Platform-Specific Notes

### Android
- Requires notification permissions (Android 13+)
- Uses AndroidX compatibility
- Minimum SDK: API 21 (Android 5.0)
- Target SDK: API 34 (Android 14)

### iOS
- Requires notification authorization
- Minimum version: iOS 12.0
- Uses UIKit underneath

### Windows
- Minimum version: Windows 11 (Build 22000)
- Notification support via Windows Notification Service

## Security Considerations

- Passwords are hashed using SHA256 (consider upgrading to bcrypt/Argon2 for production)
- Credentials stored in platform-specific secure storage
- SQLite database is local and not encrypted (consider encryption for sensitive medical data)
- No server-side authentication (local-only app)

---

**Built with .NET MAUI** | **Version 1.0.0**
