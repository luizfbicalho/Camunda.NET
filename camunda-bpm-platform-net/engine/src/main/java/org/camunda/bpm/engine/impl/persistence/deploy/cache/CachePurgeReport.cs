using System.Collections.Generic;
using System.Text;

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
namespace org.camunda.bpm.engine.impl.persistence.deploy.cache
{
	using PurgeReporting = org.camunda.bpm.engine.impl.management.PurgeReporting;


	/// <summary>
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	public class CachePurgeReport : PurgeReporting<ISet<string>>
	{

	  public const string PROCESS_DEF_CACHE = "PROC_DEF_CACHE";
	  public const string BPMN_MODEL_INST_CACHE = "BPMN_MODEL_INST_CACHE";
	  public const string CASE_DEF_CACHE = "CASE_DEF_CACHE";
	  public const string CASE_MODEL_INST_CACHE = "CASE_MODEL_INST_CACHE";
	  public const string DMN_DEF_CACHE = "DMN_DEF_CACHE";
	  public const string DMN_REQ_DEF_CACHE = "DMN_REQ_DEF_CACHE";
	  public const string DMN_MODEL_INST_CACHE = "DMN_MODEL_INST_CACHE";

	  /// <summary>
	  /// Key: cache name
	  /// Value: values
	  /// </summary>
	  internal IDictionary<string, ISet<string>> deletedCache = new Dictionary<string, ISet<string>>();

	  public virtual void addPurgeInformation(string key, ISet<string> value)
	  {
		deletedCache[key] = new HashSet<string>(value);
	  }

	  public virtual IDictionary<string, ISet<string>> PurgeReport
	  {
		  get
		  {
			return deletedCache;
		  }
	  }

	  public virtual string PurgeReportAsString
	  {
		  get
		  {
			StringBuilder builder = new StringBuilder();
			foreach (string key in deletedCache.Keys)
			{
			  builder.Append("Cache: ").Append(key).Append(" contains: ").Append(getReportValue(key)).Append("\n");
			}
			return builder.ToString();
		  }
	  }

	  public virtual ISet<string> getReportValue(string key)
	  {
		return deletedCache[key];
	  }

	  public virtual bool containsReport(string key)
	  {
		return deletedCache.ContainsKey(key);
	  }

	  public virtual bool Empty
	  {
		  get
		  {
			return deletedCache.Count == 0;
		  }
	  }
	}

}