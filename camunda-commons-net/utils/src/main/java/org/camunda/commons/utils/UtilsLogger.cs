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
namespace org.camunda.commons.utils
{
	using BaseLogger = org.camunda.commons.logging.BaseLogger;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class UtilsLogger : BaseLogger
	{

	  public const string PROJECT_CODE = "UTILS";

	  public static readonly IoUtilLogger IO_UTIL_LOGGER = BaseLogger.createLogger(typeof(IoUtilLogger), PROJECT_CODE, "org.camunda.commons.utils.io", "01");
	  public static readonly EnsureUtilLogger ENSURE_UTIL_LOGGER = BaseLogger.createLogger(typeof(EnsureUtilLogger), PROJECT_CODE, "org.camunda.commons.utils.ensure", "02");
	}

}