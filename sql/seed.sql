-- ===============================
-- Seed data for user and job tables
-- ===============================

-- Insert Users
INSERT INTO public.users (name, email, passwordhash, phone, role) VALUES
('Alice Johnson', 'alice@example.com', 'hashedpw1', '+4512345678', 'jobseeker'),
('Bob Smith', 'bob@example.com', 'hashedpw2', '+4522334455', 'employer'),
('Carol White', 'carol@example.com', 'hashedpw3', '+4533445566', 'jobseeker'),
('David Brown', 'david@example.com', 'hashedpw4', '+4544556677', 'employer'),
('Eva Green', 'eva@example.com', 'hashedpw5', '+4555667788', 'jobseeker');

-- Insert Jobs 
INSERT INTO public.job (employerid, title, company, description, location, salaryrange, category, status) VALUES
(2, 'Frontend Developer', 'CoolStartup', 'Build UIs with React.', 'Copenhagen', 'DKK 45,000–60,000/year', 'Full-time', 'active'),
(4, 'Marketing Assistant', 'MarketMakers', 'Support campaigns.', 'Aarhus', 'DKK 30,000–45,000/year', 'Part-time', 'active'),
(2, 'QA Tester', 'BugSquashers Inc.', 'Test web apps.', 'Remote', 'DKK 40,000–50,000/year', 'Full-time', 'expired'),
(4, 'Data Analyst Intern', 'DataDriven Co.', 'Analyze sales data.', 'Odense', 'DKK 25,000–30,000/year', 'Part-time', 'closed'),
(2, 'Content Writer', 'Content Creators', 'Write blog articles.', 'Copenhagen', 'DKK 35,000–45,000/year', 'Full-time', 'active');


-- ===============================
-- Seed data for resume and jobapplication tables
-- ===============================

-- Insert Resumes (assumes jobseeker IDs are 1, 3, 5)
INSERT INTO public.resume (jobseekerid, content) VALUES
(1, 'Resume content for Alice Johnson. Experienced frontend developer.'),
(3, 'Resume content for Carol White. Background in content marketing.'),
(5, 'Resume content for Eva Green. Data science and analytics focused.');

-- Insert Job Applications (uses new resume IDs: 1, 2, 3)
INSERT INTO public.jobapplication (jobid, jobseekerid, resumeid, coverletter, status) VALUES
(1, 1, 1, 'I am excited to apply for the Frontend Developer position.', 'applied'),
(2, 3, 2, 'Marketing is my passion – I believe I’d be a great fit.', 'reviewed'),
(4, 5, 3, 'This internship matches my interests in data analysis.', 'applied');

-- ===============================
-- SELECT queries to retrieve data
-- ===============================

-- All users
SELECT * FROM public.users;

-- All jobs
SELECT * FROM public.job;

-- Active jobs only
SELECT * FROM public.job WHERE status = 'active';

-- Jobs posted by Bob Smith (id = 2)
SELECT * FROM public.job WHERE employerid = 2;

-- Job titles and their location
SELECT title, location FROM public.job;

-- Employer name and their job titles
SELECT u.name AS employer_name, j.title
FROM public.users u
JOIN public.job j ON u.id = j.employerid;

-- Job count per employer
SELECT employerid, COUNT(*) AS job_count
FROM public.job
GROUP BY employerid;

-- Job applications with job and applicant info
SELECT ja.id, u.name AS jobseeker, j.title AS job_title, ja.coverletter, ja.status
FROM public.jobapplication ja
JOIN public.users u ON ja.jobseekerid = u.id
JOIN public.job j ON ja.jobid = j.id;

-- All resumes with jobseeker name
SELECT r.id, u.name AS jobseeker, r.content
FROM public.resume r
JOIN public.users u ON r.jobseekerid = u.id;
