--
-- PostgreSQL database dump
--

-- Dumped from database version 16.2
-- Dumped by pg_dump version 16.2

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: map; Type: TABLE; Schema: public; Owner: Kanishq_Mehta
--

CREATE TABLE public.map (
    id integer NOT NULL,
    columns integer NOT NULL,
    rows integer NOT NULL,
    "Name" character varying(50) NOT NULL,
    description character varying(800),
    createddate timestamp without time zone NOT NULL,
    modifieddate timestamp without time zone NOT NULL,
    issquare boolean GENERATED ALWAYS AS (((rows > 0) AND (rows = columns))) STORED
);


ALTER TABLE public.map OWNER TO "Kanishq_Mehta";

--
-- Name: map_id_seq; Type: SEQUENCE; Schema: public; Owner: Kanishq_Mehta
--

ALTER TABLE public.map ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.map_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: robotcommand; Type: TABLE; Schema: public; Owner: Kanishq_Mehta
--

CREATE TABLE public.robotcommand (
    id integer NOT NULL,
    "Name" character varying(50) NOT NULL,
    description character varying(800),
    ismovecommand boolean NOT NULL,
    createddate timestamp without time zone NOT NULL,
    modifieddate timestamp without time zone NOT NULL
);


ALTER TABLE public.robotcommand OWNER TO "Kanishq_Mehta";

--
-- Name: robotcommand_id_seq; Type: SEQUENCE; Schema: public; Owner: Kanishq_Mehta
--

ALTER TABLE public.robotcommand ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.robotcommand_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: user; Type: TABLE; Schema: public; Owner: Kanishq_Mehta
--

CREATE TABLE public."user" (
    id integer NOT NULL,
    email character varying(50) NOT NULL,
    firstname character varying(50) NOT NULL,
    lastname character varying(50) NOT NULL,
    passwordhash character varying(50) NOT NULL,
    description character varying(50),
    role character varying(50),
    createddate timestamp without time zone NOT NULL,
    modifieddate timestamp without time zone NOT NULL
);


ALTER TABLE public."user" OWNER TO "Kanishq_Mehta";

--
-- Name: user_id_seq; Type: SEQUENCE; Schema: public; Owner: Kanishq_Mehta
--

ALTER TABLE public."user" ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.user_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Data for Name: map; Type: TABLE DATA; Schema: public; Owner: Kanishq_Mehta
--

COPY public.map (id, columns, rows, "Name", description, createddate, modifieddate) FROM stdin;
1	25	25	MAP1	TEST MAP	2024-03-07 00:00:00	2024-03-07 00:00:00
2	0	0	UpdatedMaps	N/A	2024-03-08 10:24:29.209099	2024-03-24 23:33:27.062855
3	0	40	Test Map	This is a test map	2024-03-24 23:50:43.944553	2024-03-24 23:50:43.944553
4	0	32	Map3	This is again a test map	2024-03-24 23:55:25.095286	2024-03-24 23:55:25.095286
\.


--
-- Data for Name: robotcommand; Type: TABLE DATA; Schema: public; Owner: Kanishq_Mehta
--

COPY public.robotcommand (id, "Name", description, ismovecommand, createddate, modifieddate) FROM stdin;
1	PLACE	\N	f	2024-03-06 00:00:00	2024-03-06 00:00:00
2	MOVE	\N	t	2024-03-06 00:00:00	2024-03-06 00:00:00
3	RIGHT	\N	t	2024-03-06 00:00:00	2024-03-06 00:00:00
4	LEFT	\N	t	2024-03-06 00:00:00	2024-03-06 00:00:00
8	NewCommand	N/A	f	2024-03-30 14:54:27.643897	2024-03-30 14:54:27.643939
\.


--
-- Data for Name: user; Type: TABLE DATA; Schema: public; Owner: Kanishq_Mehta
--

COPY public."user" (id, email, firstname, lastname, passwordhash, description, role, createddate, modifieddate) FROM stdin;
\.


--
-- Name: map_id_seq; Type: SEQUENCE SET; Schema: public; Owner: Kanishq_Mehta
--

SELECT pg_catalog.setval('public.map_id_seq', 5, true);


--
-- Name: robotcommand_id_seq; Type: SEQUENCE SET; Schema: public; Owner: Kanishq_Mehta
--

SELECT pg_catalog.setval('public.robotcommand_id_seq', 8, true);


--
-- Name: user_id_seq; Type: SEQUENCE SET; Schema: public; Owner: Kanishq_Mehta
--

SELECT pg_catalog.setval('public.user_id_seq', 1, false);


--
-- Name: map pk_map; Type: CONSTRAINT; Schema: public; Owner: Kanishq_Mehta
--

ALTER TABLE ONLY public.map
    ADD CONSTRAINT pk_map PRIMARY KEY (id);


--
-- Name: robotcommand pk_robotcommand; Type: CONSTRAINT; Schema: public; Owner: Kanishq_Mehta
--

ALTER TABLE ONLY public.robotcommand
    ADD CONSTRAINT pk_robotcommand PRIMARY KEY (id);


--
-- PostgreSQL database dump complete
--

