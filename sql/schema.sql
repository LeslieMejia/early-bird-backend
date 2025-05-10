--EBDatabase: SQLSCRIPT

-- ENUM TYPES
CREATE TYPE public.userrole AS ENUM ('employer', 'jobseeker');
CREATE TYPE public.jobstatus AS ENUM ('active', 'expired', 'closed');
CREATE TYPE public.applicationstatus AS ENUM ('applied', 'reviewed', 'interview', 'rejected', 'accepted');

-- USER TABLE
--Renamed public.user to users to match backend expectations and avoid conflicts with reserved keywords in PostgreSQL.
CREATE TABLE public.users (
    id INTEGER NOT NULL,
    name TEXT NOT NULL,
    email TEXT NOT NULL,
    passwordhash TEXT,
    phone TEXT,
    role public.userrole NOT NULL
);

-- Sequence for user.id
CREATE SEQUENCE public.user_id_seq
    AS INTEGER START WITH 1 INCREMENT BY 1 NO MINVALUE NO MAXVALUE CACHE 1;

ALTER SEQUENCE public.user_id_seq OWNED BY public.user.id;

ALTER TABLE ONLY public.users
    ALTER COLUMN id SET DEFAULT nextval('public.user_id_seq'::regclass);

ALTER TABLE ONLY public.users
    ADD CONSTRAINT user_pkey PRIMARY KEY (id);

-- JOB TABLE
CREATE TABLE public.job (
    id INTEGER NOT NULL,
    employerid INTEGER NOT NULL,
    title TEXT NOT NULL,
    description TEXT,
    location TEXT,
    salaryrange TEXT,
    category TEXT,
    company TEXT,
    status public.jobstatus NOT NULL
);

-- Sequence for job.id
CREATE SEQUENCE public.job_id_seq
    AS INTEGER START WITH 1 INCREMENT BY 1 NO MINVALUE NO MAXVALUE CACHE 1;

ALTER SEQUENCE public.job_id_seq OWNED BY public.job.id;

ALTER TABLE ONLY public.job
    ALTER COLUMN id SET DEFAULT nextval('public.job_id_seq'::regclass);

ALTER TABLE ONLY public.job
    ADD CONSTRAINT job_pkey PRIMARY KEY (id);

-- RESUME TABLE
CREATE TABLE public.resume (
    id INTEGER NOT NULL,
    jobseekerid INTEGER NOT NULL,
    content TEXT NOT NULL
);

-- Sequence for resume.id
CREATE SEQUENCE public.resume_id_seq
    AS INTEGER START WITH 1 INCREMENT BY 1 NO MINVALUE NO MAXVALUE CACHE 1;

ALTER SEQUENCE public.resume_id_seq OWNED BY public.resume.id;

ALTER TABLE ONLY public.resume
    ALTER COLUMN id SET DEFAULT nextval('public.resume_id_seq'::regclass);

ALTER TABLE ONLY public.resume
    ADD CONSTRAINT resume_pkey PRIMARY KEY (id);

-- JOBAPPLICATION TABLE
CREATE TABLE public.jobapplication (
    id INTEGER NOT NULL,
    jobid INTEGER NOT NULL,
    jobseekerid INTEGER NOT NULL,
    resumeid INTEGER,
    coverletter TEXT, 
    status public.applicationstatus NOT NULL
);

-- Sequence for jobapplication.id
CREATE SEQUENCE public.jobapplication_id_seq
    AS INTEGER START WITH 1 INCREMENT BY 1 NO MINVALUE NO MAXVALUE CACHE 1;

ALTER SEQUENCE public.jobapplication_id_seq OWNED BY public.jobapplication.id;

ALTER TABLE ONLY public.jobapplication
    ALTER COLUMN id SET DEFAULT nextval('public.jobapplication_id_seq'::regclass);

ALTER TABLE ONLY public.jobapplication
    ADD CONSTRAINT jobapplication_pkey PRIMARY KEY (id);

-- FOREIGN KEYS
ALTER TABLE ONLY public.job
    ADD CONSTRAINT fk_job_employerid FOREIGN KEY (employerid) REFERENCES public.user(id) MATCH SIMPLE
    ON UPDATE NO ACTION ON DELETE CASCADE NOT VALID;

ALTER TABLE ONLY public.resume
    ADD CONSTRAINT fk_resume_jobseekerid FOREIGN KEY (jobseekerid) REFERENCES public.user(id) MATCH SIMPLE
    ON UPDATE NO ACTION ON DELETE CASCADE NOT VALID;

ALTER TABLE ONLY public.jobapplication
    ADD CONSTRAINT fk_jobapplication_jobid FOREIGN KEY (jobid) REFERENCES public.job(id) MATCH SIMPLE
    ON UPDATE NO ACTION ON DELETE CASCADE NOT VALID;

ALTER TABLE ONLY public.jobapplication
    ADD CONSTRAINT fk_jobapplication_jobseekerid FOREIGN KEY (jobseekerid) REFERENCES public.user(id) MATCH SIMPLE
    ON UPDATE NO ACTION ON DELETE CASCADE NOT VALID;

ALTER TABLE ONLY public.jobapplication
    ADD CONSTRAINT fk_jobapplication_resumeid FOREIGN KEY (resumeid) REFERENCES public.resume(id) MATCH SIMPLE
    ON UPDATE NO ACTION ON DELETE SET NULL NOT VALID;
