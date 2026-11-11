# Intelligent Integrated Driving & Vehicle License Management System (IIDVLMS)

A comprehensive, enterprise-level desktop application designed to automate and streamline the operations of driving and vehicle license departments. Developed as a final-year Software Engineering graduation project at **Al Sham Private University (ASPU)**.

> **Project Status:** This project is currently in active development. New modules are being added continuously following the Agile methodology.

## 📌 The Problem (Why this system exists)
In many developing countries, driving and vehicle license departments suffer from severe operational inefficiencies. The core issues include:
- **Data Fragmentation:** Heavy reliance on paper-based records or disconnected, isolated desktop applications. This leads to data duplication, loss of history, and extreme difficulty in retrieving accurate information.
- **Lack of Automated Validation:** Human errors are common because there is no automated system to check if an applicant meets the strict requirements (e.g., age limits for specific license classes, medical vision tests, or previous unpaid violations).
- **Inefficient Violation & Impoundment Management:** Tracking unpaid fines, linking them to specific drivers and vehicles, and managing vehicle impoundment (detaining/releasing) is a manual and error-prone process.
- **Poor Reporting:** Management lacks the tools to generate real-time reports, making it impossible to know how many licenses were issued, how many vehicles are currently impounded, or which employees are processing the most applications.

## 💡 The Solution (What IIDVLMS provides)
The IIDVLMS aims to replace these fragmented systems with a **centralized, automated, and highly secure desktop solution**. By integrating all license and vehicle management processes into one cohesive system, it ensures data integrity, eliminates human errors, and speeds up service delivery for citizens.

## ✨ Key Features & Modules
The system is divided into several robust modules, each handling a critical part of the department's workflow:

### 1. People & User Management
- Complete CRUD (Create, Read, Update, Delete) operations for citizens and system users.
- Stores comprehensive personal data including national ID, full name, date of birth, address, contact info, and personal photos.
- Prevents duplicate records by validating the National ID.
- **Role-Based Access Control (RBAC):** Manages system users with strict permissions (Admin, Employee, Viewer) to ensure system security and data privacy.

### 2. Driver & License Management
- **7 License Classes:** Manages various license classes (e.g., Small Motorcycle, Ordinary Vehicle, Heavy Vehicle) with specific age requirements and fees.
- **Issuance & Renewal:** Handles first-time license issuance, renewals, and replacements for lost or damaged licenses.
- **International Licenses:** Manages the issuance of international driving permits for eligible drivers.
- **License Detainment:** Tracks and manages the detention and release of driving licenses based on traffic violations.

### 3. Application Processing & Testing
- Manages **8 different types of applications** (New, Renewal, Lost, Damaged, etc.).
- **Testing Module:** Tracks the three mandatory test stages:
  1. Vision Test (Medical check).
  2. Theoretical Test (Written exam).
  3. Practical Test (Driving exam).
- Applicants cannot receive a license until they pass all required tests, with the system automatically validating their eligibility.

### 4. Vehicle & Violation Management
- Registers vehicles, links them to their owners, and tracks ownership transfers.
- **Traffic Violations:** Records violations, calculates fines, and links them to the driver's record. Drivers with unpaid fines cannot renew their licenses.
- **Vehicle Impoundment:** Manages the vehicle detention process and ensures fines are paid before releasing the vehicle.

### 5. Comprehensive Reporting & Audit Logs
- Generates detailed reports for management (e.g., issued licenses, detained vehicles).
- **Audit Log:** Tracks every single action performed in the system (Who did what, and when), ensuring full transparency and accountability.

## 🏗️ Software Architecture (3-Tier Architecture)
The system is built on a strong architectural foundation, strictly following the **3-Tier Architecture** pattern to separate concerns and ensure scalability:

- **Presentation Layer (UI):** Built using C# Windows Forms. Features a modern, responsive interface with reusable User Controls (e.g., Person Details control) to reduce redundancy. The UI does not contain any business logic; it only displays data and captures user input.
- **Business Logic Layer (BLL):** Contains the core entities (e.g., `clsPerson`, `clsApplication`, `clsLicense`) and the business rules. It acts as a bridge between the UI and the database. For example, it validates age and test results before allowing an application to proceed.
- **Data Access Layer (DAL):** Manages all communication with the SQL Server database using ADO.NET. It executes queries, stored procedures, and ensures data is retrieved or saved efficiently. The DAL is completely isolated from the UI.

## 🛠️ Technology Stack
- **Programming Language:** C#
- **Framework:** .NET Framework (Windows Forms)
- **Database:** Microsoft SQL Server
- **Data Access Technology:** ADO.NET
- **Version Control:** Git & GitHub
- **Development Methodology:** Agile Model (Sprints and Continuous Integration)

## 📂 Project Structure
The repository is organized into three main projects to reflect the 3-Tier architecture:

```text
IIDVLMS-Desktop-App/
│
├── DVLD/                     # Presentation Layer (UI Forms, Controls, Resources)
├── DVLD_BussnessLayer/       # Business Logic Layer (Entities & Rules)
└── DVLD_DataAccessLayer/     # Data Access Layer (SQL Connections, Queries, Settings)
