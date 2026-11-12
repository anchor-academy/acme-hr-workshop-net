using Api.Models;

namespace Api.Data;

/// <summary>Fifteen synthetic HR policies for Acme HR Sdn Bhd. Fictional, for training only.</summary>
public static class PolicyDocs
{
    public static readonly Policy[] All =
    [
        new() { Id = "POL-01", Title = "Annual leave entitlement", Body = "Permanent staff get 14 days of annual leave per calendar year. Staff with five or more years of service get 18 days. Leave is counted in working days, Monday to Friday. Weekends and public holidays inside a leave period are not deducted. New joiners receive leave pro-rated by month of joining." },
        new() { Id = "POL-02", Title = "Applying for leave", Body = "Apply in Acme HR at least 3 working days before the leave starts. Requests of up to 3 working days are approved automatically if the balance is enough. Requests longer than 3 working days need approval from the direct manager. Overlapping requests are not allowed." },
        new() { Id = "POL-03", Title = "Half-day leave", Body = "Staff may take annual leave as a half day, either AM (9:00 to 13:00) or PM (14:00 to 18:00). A half day deducts 0.5 day from the balance. A half day must start and end on the same working day. Two half days on the same date count as one full day." },
        new() { Id = "POL-04", Title = "Carry forward", Body = "Up to 5 unused annual leave days may be carried forward to the next year. Carried-forward days must be used by 31 March, after which they lapse. Carried-forward days may be taken as full days or half days. Days above the 5-day cap lapse on 31 December unless HR approves an exception in writing." },
        new() { Id = "POL-05", Title = "Medical leave", Body = "Staff get 14 days of paid medical leave per year, or 18 days after five years of service. A medical certificate (MC) from a registered clinic is required for every day of medical leave. Upload the MC in Acme HR within 2 working days. Medical leave does not reduce annual leave." },
        new() { Id = "POL-06", Title = "Hospitalisation leave", Body = "Staff get up to 60 days of hospitalisation leave per year, including medical leave days already taken. A hospital discharge note is required. HR will contact the staff member to confirm return-to-work dates." },
        new() { Id = "POL-07", Title = "Emergency leave", Body = "Emergency leave may be applied on the same day for urgent family matters. It is deducted from annual leave. The staff member must inform the manager by phone or chat before 10:00 and apply in Acme HR within 1 working day." },
        new() { Id = "POL-08", Title = "Unpaid leave", Body = "Unpaid leave of up to 10 working days a year may be approved by the department head when annual leave is used up. Unpaid leave above 10 days needs HR director approval." },
        new() { Id = "POL-09", Title = "Expense claims", Body = "Business expenses are claimed in Acme HR within 30 days of the expense, with an itemised receipt or tax invoice. Claims above RM500 need manager approval. Totals must equal subtotal plus tax. Personal items are not claimable." },
        new() { Id = "POL-10", Title = "Travel and meals", Body = "Meal allowance on business travel is RM60 per day in Malaysia. Economy airfare only. Hotel limit is RM300 per night in Kuala Lumpur and RM250 elsewhere. Mileage is claimed at RM0.60 per km with a route map." },
        new() { Id = "POL-11", Title = "Remote work", Body = "Staff may work remotely up to 2 days per week with manager agreement. Core hours are 10:00 to 16:00. Remote work is not leave and does not need a leave request. Staff must be reachable on chat during core hours." },
        new() { Id = "POL-12", Title = "Personal data (PDPA)", Body = "Acme HR handles personal data under the Personal Data Protection Act 2010. Staff data is used only for HR purposes. Medical documents are visible only to HR and the employee. Do not paste staff personal data into tools that are not approved by IT." },
        new() { Id = "POL-13", Title = "Public holidays", Body = "Staff follow the public holidays of the state where they are based. If a public holiday falls on a Sunday, the next working day is a replacement holiday. Public holidays inside a leave period are not deducted from annual leave." },
        new() { Id = "POL-14", Title = "Use of AI tools", Body = "Only company-approved AI tools may be used for work. AI agents may answer HR questions and prepare requests, but approvals of leave above 3 days, claims above RM500, and any change to pay must be made by a person. AI answers must cite the policy they rely on." },
        new() { Id = "POL-15", Title = "Resignation and leave balance", Body = "On resignation, unused annual leave may be used during the notice period with manager approval, or paid out at the daily rate. Negative leave balance is deducted from the final salary." },
    ];
}
