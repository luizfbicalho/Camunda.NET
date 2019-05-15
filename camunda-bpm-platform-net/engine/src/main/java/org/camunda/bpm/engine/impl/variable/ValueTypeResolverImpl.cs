using System.Collections.Generic;

/*
 * Copyright Camunda Services GmbH and/or licensed to Camunda Services GmbH
 * under one or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information regarding copyright
 * ownership. Camunda licenses this file to you under the Apache License,
 * Version 2.0; you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace org.camunda.bpm.engine.impl.variable
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.variable.type.ValueType.BOOLEAN;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.variable.type.ValueType.BYTES;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.variable.type.ValueType.DATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.variable.type.ValueType.DOUBLE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.variable.type.ValueType.FILE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.variable.type.ValueType.INTEGER;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.variable.type.ValueType.LONG;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.variable.type.ValueType.NULL;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.variable.type.ValueType.NUMBER;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.variable.type.ValueType.OBJECT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.variable.type.ValueType.SHORT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.variable.type.ValueType.STRING;


	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;
	using ValueTypeResolver = org.camunda.bpm.engine.variable.type.ValueTypeResolver;

	/// <summary>
	/// Resolves ValueType by name.
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ValueTypeResolverImpl : ValueTypeResolver
	{

	  protected internal IDictionary<string, ValueType> knownTypes = new Dictionary<string, ValueType>();

	  public ValueTypeResolverImpl()
	  {
		addType(BOOLEAN);
		addType(BYTES);
		addType(DATE);
		addType(DOUBLE);
		addType(INTEGER);
		addType(LONG);
		addType(NULL);
		addType(SHORT);
		addType(STRING);
		addType(OBJECT);
		addType(NUMBER);
		addType(FILE);
	  }

	  public virtual void addType(ValueType type)
	  {
		knownTypes[type.Name] = type;
	  }

	  public virtual ValueType typeForName(string typeName)
	  {
		return knownTypes[typeName];
	  }

	  public virtual ICollection<ValueType> getSubTypes(ValueType type)
	  {
		IList<ValueType> types = new List<ValueType>();

		ISet<ValueType> validParents = new HashSet<ValueType>();
		validParents.Add(type);

		foreach (ValueType knownType in knownTypes.Values)
		{
		  if (validParents.Contains(knownType.Parent))
		  {
			validParents.Add(knownType);

			if (!knownType.Abstract)
			{
			  types.Add(knownType);
			}
		  }
		}

		return types;
	  }

	}

}