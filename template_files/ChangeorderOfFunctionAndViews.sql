  -- Supports Stored Proc, View, User Function, User Table

    WITH CTE_DependentObjects AS
    (
        SELECT DISTINCT 
        b.object_id AS UsedByObjectId, 
		SCHEMA_NAME(b.schema_id) AS UseByObjectSchema,
        b.name AS UsedByObjectName, b.type AS UsedByObjectType, 
        c.object_id AS DependentObjectId, 
		SCHEMA_NAME(c.schema_id) AS DependentObjectSchema,
        c.name AS DependentObjectName , c.type AS DependenObjectType
        FROM  sys.objects b
        LEFT OUTER JOIN sys.sysdepends a ON a.id = b.object_id 
        left outer JOIN sys.objects c ON a.depid = c.object_id AND c.type IN ('fn', 'if', 'tf') -- , USE 'V' for views, ('fn', 'if', 'tf') for functions, 'P' for procedures etc
		where b.type IN ('fn', 'if', 'tf')
		AND SCHEMA_NAME(b.schema_id) = 'tSQLt'
      
    ),
    CTE_DependentObjects2 AS
    (
       SELECT 
          UsedByObjectId, UsedByObjectName, UsedByObjectType,
          DependentObjectId, DependentObjectName, DependenObjectType, UseByObjectSchema, DependentObjectSchema,
          1 AS Level
       FROM CTE_DependentObjects a
       WHERE a.DependentObjectId is NULL
	   --WHERE a.UsedByObjectName = @PARAM_OBJECT_NAME
       UNION ALL 
       SELECT 
          a.UsedByObjectId, a.UsedByObjectName, a.UsedByObjectType,
          a.DependentObjectId, a.DependentObjectName, a.DependenObjectType, a.UseByObjectSchema, a.DependentObjectSchema,
          (b.Level + 1) AS Level
       FROM CTE_DependentObjects a
       INNER JOIN  CTE_DependentObjects2 b 
         ON b.UsedByObjectId = a.DependentObjectId
		 --and a.UsedByObjectName = b.DependentObjectName AND a.UseByObjectSchema = b.DependentObjectSchema
    )
    SELECT DISTINCT '    <include file="'+UseByObjectSchema+'.'+UsedByObjectName+'.sql" relativeToChangelogFile="true" />', m.Level
	FROM CTE_DependentObjects2 m
	WHERE NOT EXISTS(
		SELECT 1 FROM CTE_DependentObjects2 m2 
					 WHERE m2.level > m.level 
					 AND m.UsedByObjectId = m2.UsedByObjectId
		  )

    ORDER BY m.Level   
	OPTION (MAXRECURSION 32767);