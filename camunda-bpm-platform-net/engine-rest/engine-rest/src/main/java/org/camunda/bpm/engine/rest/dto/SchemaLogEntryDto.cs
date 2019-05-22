using System;
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
namespace org.camunda.bpm.engine.rest.dto
{

	using SchemaLogEntry = org.camunda.bpm.engine.management.SchemaLogEntry;

	/// <summary>
	/// @author Miklas Boskamp
	/// 
	/// </summary>
	public class SchemaLogEntryDto
	{

	  private string id;
	  private DateTime timestamp;
	  private string version;

	  public static IList<SchemaLogEntryDto> fromSchemaLogEntries(IList<SchemaLogEntry> entries)
	  {
		IList<SchemaLogEntryDto> dtos = new List<SchemaLogEntryDto>();
		foreach (SchemaLogEntry entry in entries)
		{
		  dtos.Add(new SchemaLogEntryDto(entry.Id, entry.Timestamp, entry.Version));
		}
		return dtos;
	  }

	  public SchemaLogEntryDto(string id, DateTime timestamp, string version)
	  {
		this.id = id;
		this.timestamp = timestamp;
		this.version = version;
	  }

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
		  set
		  {
			this.id = value;
		  }
	  }


	  public virtual DateTime Timestamp
	  {
		  get
		  {
			return timestamp;
		  }
		  set
		  {
			this.timestamp = value;
		  }
	  }


	  public virtual string Version
	  {
		  get
		  {
			return version;
		  }
		  set
		  {
			this.version = value;
		  }
	  }

	}

}