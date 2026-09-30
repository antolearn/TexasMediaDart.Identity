/*
    SeedTermsDocuments.sql

    Seeds versioned TexasDart Terms and Conditions.

    IMPORTANT:
    - Published Terms versions are immutable.
    - Never update the Content of an existing Version.
    - Create a new Version whenever the Terms change.
    - DocumentHash is calculated from the exact NVARCHAR content stored below.
*/

DECLARE @Version NVARCHAR(50) = N'2026.09';
DECLARE @Title NVARCHAR(200) = N'TexasDart Terms and Conditions';
DECLARE @EffectiveUtc DATETIME2(7) = '2026-09-29T00:00:00';

DECLARE @Content NVARCHAR(MAX) = N'
TexasDart Terms and Conditions

Effective Date: September 29, 2026
Version: 2026.09

These Terms and Conditions ("Terms") govern your access to and use of TexasDart, including its websites, applications, APIs, software, services, and related features (collectively, the "Service"), provided by Vetri Software Solutions LLC ("Company," "we," "us," or "our").

By creating an account, clicking "I Agree," accessing, or using the Service, you acknowledge that you have read, understood, and agree to be bound by these Terms. If you do not agree to these Terms, you must not create an account or use the Service.

1. ELIGIBILITY AND AUTHORITY

You must have the legal capacity to enter into a binding agreement to use the Service.

If you create or use an account on behalf of a company, organization, or other legal entity, you represent that you have authority to bind that organization to these Terms. In that case, references to "you" include that organization.

You are responsible for ensuring that your use of the Service complies with laws and regulations applicable to you and your organization.

2. ACCOUNT REGISTRATION

You agree to provide accurate, current, and complete information when creating and maintaining your account.

You are responsible for maintaining the confidentiality and security of your account credentials and for activities conducted through your account to the extent permitted by applicable law.

You must promptly notify us if you reasonably believe that your account or credentials have been compromised or used without authorization.

You may not impersonate another person, create an account using information you are not authorized to use, or knowingly provide false registration information.

3. AUTHORIZED USE OF TEXASDART

TexasDart is provided for legitimate business and organizational purposes.

Subject to these Terms and any applicable subscription agreement, we grant you a limited, non-exclusive, non-transferable, revocable right to access and use the Service.

You may not use the Service to violate applicable law; infringe intellectual-property, privacy, confidentiality, or other rights; distribute malware or malicious code; interfere with the security or operation of the Service; gain unauthorized access to systems or accounts; conduct fraudulent activities; abuse APIs or system resources; circumvent access, licensing, or security controls; or use the Service in a manner reasonably likely to damage the Service or other users.

4. USER AND ORGANIZATION RESPONSIBILITIES

You are responsible for information, files, records, configurations, instructions, and other content that you or your authorized users submit to the Service ("Customer Data").

You represent that you have the rights, permissions, notices, and consents necessary to provide Customer Data to the Service and to permit us to process it as necessary to provide the Service.

Your organization is responsible for managing its users, roles, permissions, access rights, licenses, and administrative settings.

We are not responsible for consequences resulting from permissions, configurations, approvals, deletions, modifications, or other actions performed by users whom your organization has authorized, except to the extent required by applicable law.

5. CUSTOMER DATA

As between you and the Company, you retain your rights in Customer Data.

You grant us the rights necessary to host, store, transmit, back up, process, reproduce, and otherwise handle Customer Data solely as reasonably necessary to operate, secure, maintain, support, and improve the Service, fulfill our contractual obligations, and comply with applicable law.

We do not acquire ownership of Customer Data merely because it is submitted to TexasDart.

You are responsible for maintaining copies or exports of data that your organization considers necessary, subject to any backup, retention, or recovery commitments expressly included in your applicable service agreement.

6. PRIVACY AND SECURITY

Our collection and use of personal information will also be governed by our Privacy Policy.

We may implement administrative, technical, and organizational safeguards designed to protect information processed through the Service. However, no internet-based service, transmission method, or storage system can be guaranteed to be completely secure.

You are responsible for using reasonable security practices, including protecting credentials, assigning appropriate permissions, removing access when users no longer require it, and keeping devices and systems used to access TexasDart reasonably secure.

7. INTELLECTUAL PROPERTY

TexasDart and its software, source code, object code, architecture, interfaces, designs, graphics, documentation, trademarks, logos, workflows, and other proprietary materials are owned by or licensed to Vetri Software Solutions LLC and are protected by applicable intellectual-property laws.

Except for rights expressly granted under these Terms, no ownership rights in TexasDart are transferred to you.

You may not copy, sell, sublicense, distribute, modify, reverse engineer, decompile, disassemble, create unauthorized derivative works from, or attempt to discover the source code of the Service except where such restriction is prohibited by applicable law.

8. FEEDBACK

If you voluntarily provide suggestions, ideas, recommendations, or other feedback concerning TexasDart, you grant the Company a worldwide, perpetual, irrevocable, royalty-free right to use that feedback without obligation or compensation to you.

This provision does not transfer ownership of your Customer Data to us.

9. THIRD-PARTY SERVICES

TexasDart may interact with third-party platforms, authentication providers, cloud services, APIs, websites, or other products.

Those services may be governed by separate terms and privacy policies.

To the extent permitted by law, the Company is not responsible for the availability, operation, security, accuracy, or acts or omissions of independent third-party services that are outside our control.

10. SUBSCRIPTION, FEES, AND TAXES

Certain TexasDart functionality may require a paid subscription or license.

Pricing, billing periods, usage limits, renewal terms, and included features will be disclosed through the applicable order, subscription, or purchasing process.

You agree to pay applicable charges and taxes in accordance with the terms presented when purchasing the Service.

We may modify pricing for future subscription periods upon applicable notice, subject to contractual commitments and applicable law.

11. SERVICE AVAILABILITY AND MODIFICATIONS

We intend to operate and maintain TexasDart reliably, but continuous or error-free availability is not guaranteed unless an applicable written service-level agreement expressly provides otherwise.

The Service may occasionally be unavailable because of maintenance, upgrades, security events, infrastructure problems, third-party failures, events outside our reasonable control, or other operational circumstances.

We may modify, update, replace, or discontinue features as the Service evolves, subject to contractual obligations and applicable law.

12. SUSPENSION AND TERMINATION

We may suspend or restrict access when reasonably necessary to protect the Service or other users, address a security threat, investigate suspected fraud or unlawful activity, respond to legal requirements, address material violations of these Terms, or address overdue amounts under an applicable paid agreement.

Where reasonably practicable and legally permitted, we may provide notice and an opportunity to remedy a violation before terminating an account for breach.

You may stop using the Service subject to your applicable subscription or contractual obligations.

13. DISCLAIMER OF WARRANTIES

TO THE MAXIMUM EXTENT PERMITTED BY APPLICABLE LAW, AND EXCEPT FOR WARRANTIES EXPRESSLY PROVIDED IN A SEPARATE WRITTEN AGREEMENT, THE SERVICE IS PROVIDED "AS IS" AND "AS AVAILABLE."

THE COMPANY DISCLAIMS WARRANTIES THAT MAY OTHERWISE APPLY, INCLUDING IMPLIED WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE, TITLE, AND NON-INFRINGEMENT, TO THE EXTENT SUCH WARRANTIES MAY LAWFULLY BE DISCLAIMED.

We do not warrant that the Service will always be uninterrupted, completely secure, error-free, or suitable for every particular business purpose.

14. LIMITATION OF LIABILITY

TO THE MAXIMUM EXTENT PERMITTED BY APPLICABLE LAW, THE COMPANY AND ITS OWNERS, OFFICERS, EMPLOYEES, CONTRACTORS, AFFILIATES, AND AGENTS WILL NOT BE LIABLE FOR INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, PUNITIVE, OR CONSEQUENTIAL DAMAGES, OR FOR LOSS OF PROFITS, REVENUE, BUSINESS OPPORTUNITIES, GOODWILL, OR DATA, ARISING FROM OR RELATED TO THE SERVICE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGES.

TO THE MAXIMUM EXTENT PERMITTED BY APPLICABLE LAW, THE COMPANY''S AGGREGATE LIABILITY ARISING OUT OF OR RELATING TO THE SERVICE OR THESE TERMS WILL NOT EXCEED THE AMOUNTS PAID BY THE CUSTOMER TO THE COMPANY FOR THE SERVICE DURING THE TWELVE MONTHS IMMEDIATELY PRECEDING THE EVENT GIVING RISE TO THE CLAIM.

IF THE CUSTOMER HAS NOT PAID THE COMPANY FOR THE SERVICE, ANY ALTERNATIVE LIABILITY CAP WILL BE SUBJECT TO THE APPLICABLE AGREEMENT AND APPLICABLE LAW.

15. INDEMNIFICATION

To the extent permitted by applicable law, you agree to defend, indemnify, and hold harmless the Company and its affiliates, officers, employees, contractors, and agents from third-party claims, damages, liabilities, losses, judgments, costs, and reasonable attorneys'' fees arising from your unlawful use of the Service, Customer Data that infringes or violates third-party rights, your material violation of these Terms, or misuse of the Service by users acting under your organization''s authorization.

The Company''s right to indemnification will be subject to applicable law and any separate written agreement between the parties.

16. BUSINESS DECISIONS AND DATA ACCURACY

TexasDart may provide workflows, calculations, reports, dashboards, notifications, approvals, automation, or other information intended to assist users.

You remain responsible for reviewing information and making your organization''s business, financial, operational, compliance, advertising, billing, and other decisions.

Unless expressly agreed otherwise in writing, TexasDart is not a substitute for professional legal, accounting, tax, financial, or regulatory advice.

17. FORCE MAJEURE

Neither party will be responsible for failure or delay caused by circumstances beyond its reasonable control to the extent permitted by applicable law, including natural disasters, widespread internet or telecommunications failures, governmental actions, labor disruptions, war, terrorism, civil disturbances, epidemics, or failures of critical third-party infrastructure.

This provision does not excuse payment obligations already due unless otherwise required by law or agreed in writing.

18. CHANGES TO THESE TERMS

We may update these Terms from time to time.

When a material change requires renewed acceptance, we may present the updated Terms and require affirmative acceptance before continued use of affected portions of the Service.

The effective date and version will identify the Terms applicable to a particular acceptance.

19. GOVERNING LAW AND DISPUTES

These Terms will be governed by the laws of the State of Texas, without regard to conflict-of-law principles, except where applicable law requires otherwise.

The venue and dispute-resolution procedures applicable to a particular customer may be further specified in a separately executed agreement.

20. SEVERABILITY

If a provision of these Terms is determined to be invalid or unenforceable, the remaining provisions will remain effective to the extent permitted by law, and the affected provision will be enforced to the maximum extent legally permissible.

21. ENTIRE AGREEMENT

These Terms, together with the Privacy Policy and any applicable order form, subscription agreement, data-processing agreement, or other written agreement expressly incorporated into them, constitute the agreement governing use of the Service.

If a separately executed agreement expressly states that it controls over these Terms, that agreement will control to the extent of the conflict.

22. CONTACT INFORMATION

Questions concerning these Terms may be directed to:

Vetri Software Solutions LLC
Texas, United States
Email: legal@vetrisoft.com
';

DECLARE @DocumentHash NVARCHAR(64) =
    CONVERT(
        NVARCHAR(64),
        HASHBYTES('SHA2_256', CONVERT(VARBINARY(MAX), @Content)),
        2
    );

IF NOT EXISTS
(
    SELECT 1
    FROM [dbo].[TermsDocuments]
    WHERE [Version] = @Version
)
BEGIN
    /*
        There can be only one current Terms document.
        The filtered unique index also enforces this at database level.
    */
    UPDATE [dbo].[TermsDocuments]
    SET [IsCurrent] = 0
    WHERE [IsCurrent] = 1;

    INSERT INTO [dbo].[TermsDocuments]
    (
        [Version],
        [Title],
        [Content],
        [DocumentHash],
        [EffectiveUtc],
        [IsCurrent]
    )
    VALUES
    (
        @Version,
        @Title,
        @Content,
        @DocumentHash,
        @EffectiveUtc,
        1
    );
END;