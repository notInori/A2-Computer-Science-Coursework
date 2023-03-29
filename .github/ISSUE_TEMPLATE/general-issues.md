---
name: General Issues
about: Use this to report general issues with the program.
title: ''
labels: ''
assignees: ''

---

## Type of Issue
SQL Injection Vulnerability

## Issue Origin
**Form:** frmLogin.vb
**Class:** AuthLogin


## Issue
Although the inputs of the username and passwords fields are entered within a controlled statement. The use of speech marks within these inputs can allow users to insert extra arguements into the sql commands that are run to retreive the data that is needed to verify the current user.
 
## Recommended Solution
Inputs that are passed into these fields must be cleansed for any speechmarks. This will prevent their inputs from adding unexpected arguments into the sql query that is executed and therefore risk leaking parts of the database.
